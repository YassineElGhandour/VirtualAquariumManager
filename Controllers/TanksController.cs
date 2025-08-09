using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualAquariumManager.Data;
using VirtualAquariumManager.Models;
using VirtualAquariumManager.ViewModels;

namespace VirtualAquariumManager.Controllers
{
    [Authorize]
    public class TanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tanks
        public async Task<IActionResult> Index(string SearchString, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            
            var query = _context.Tank.AsQueryable();

            decimal? asDecimal = null;
            if (decimal.TryParse(SearchString, out var dec))
            {
                asDecimal = dec;
            }

            DateTime? asDate = null;
            if (DateTime.TryParse(SearchString, out var dt))
            {
                asDate = dt;
            }

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                SearchString = SearchString.Trim();
                var pattern = $"%{SearchString}%";

                query = query.Where(t =>
                    EF.Functions.Like(t.Shape!, pattern)
                    || EF.Functions.Like(t.WaterQuality!.WaterType!, pattern)
                    || (asDecimal.HasValue && t.Size == asDecimal.Value)
                    || (asDecimal.HasValue && t.WaterQuality.PhLevel == asDecimal.Value)
                    || (asDecimal.HasValue && t.WaterQuality.AmmoniaLevel == asDecimal.Value)
                    || (asDate.HasValue && t.CreatedDate.Date == asDate.Value.Date)
                    || (asDate.HasValue && t.WaterQuality.CreatedDate.Date == asDate.Value.Date)
                );
            }

            var tanks = await query
                        .OrderBy(t => t.CreatedDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(t => new TankWaterQualityData
                        {
                            TankId = t.Id,
                            Shape = t.Shape!,
                            Size = t.Size,
                            WaterQuality = t.WaterQuality!,
                            PhLevel = t.WaterQuality!.PhLevel,
                            Temperature = t.WaterQuality.Temperature,
                            AmmoniaLevel = t.WaterQuality.AmmoniaLevel,
                            WaterType = t.WaterQuality.WaterType,
                            CreatedDate = t.CreatedDate,
                            FishCount = t.Fish!.Count
                        })
                        .ToListAsync();
            var totalTanksCount = await query.CountAsync();

            ViewBag.CurrentSearchString = SearchString;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalTanksCount;

            return View(tanks);
        }

        // GET: Tanks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tank = await _context.Tank
                        .Select(t => new TankWaterQualityData
                        {
                            TankId = t.Id,
                            Shape = t.Shape!,
                            Size = t.Size,
                            WaterQuality = t.WaterQuality!,
                            PhLevel = t.WaterQuality!.PhLevel,
                            Temperature = t.WaterQuality.Temperature,
                            AmmoniaLevel = t.WaterQuality.AmmoniaLevel,
                            WaterType = t.WaterQuality.WaterType,
                            CreatedDate = t.CreatedDate
                        })
                        .FirstOrDefaultAsync(t => t.TankId == id);

            if (tank == null)
            {
                return NotFound();
            }

            return View(tank);
        }

        // GET: Tanks/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tank tank)
        {
            if (ModelState.IsValid)
            {
                tank.Id = Guid.NewGuid();
                _context.Add(tank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tank);
        }

        // GET: Tanks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tank = await _context.Tank
                    .Select(t => new TankWaterQualityData
                    {
                        TankId = t.Id,
                        Shape = t.Shape!,
                        Size = t.Size,
                        WaterQuality = t.WaterQuality!,
                        PhLevel = t.WaterQuality!.PhLevel,
                        Temperature = t.WaterQuality.Temperature,
                        AmmoniaLevel = t.WaterQuality.AmmoniaLevel,
                        WaterType = t.WaterQuality.WaterType,
                        CreatedDate = t.CreatedDate
                    })
                    .FirstOrDefaultAsync(t => t.TankId == id);

            if (tank == null)
            {
                return NotFound();
            }
            return View(tank);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, TankWaterQualityData TankWaterQualityModal)
        {
            if (!ModelState.IsValid)
            {
                return View(TankWaterQualityModal);
            }

            var tank = await _context.Tank
                        .Include(t => t.WaterQuality)
                        .FirstOrDefaultAsync(t => t.Id == id);

            if (tank == null)
            {
                return NotFound();
            }

            try
            {
                tank.Shape = TankWaterQualityModal.Shape;
                tank.Size = TankWaterQualityModal.Size;
                tank.WaterQuality.PhLevel = TankWaterQualityModal.PhLevel;
                tank.WaterQuality.Temperature = TankWaterQualityModal.Temperature;
                tank.WaterQuality.AmmoniaLevel = TankWaterQualityModal.AmmoniaLevel;
                tank.WaterQuality.WaterType = TankWaterQualityModal.WaterType;

                _context.Update(tank);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TankExists(TankWaterQualityModal.TankId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(
                nameof(Details),
                new { id = TankWaterQualityModal.TankId }
            );
        }

        // GET: Tanks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tank = await _context.Tank
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tank == null)
            {
                return NotFound();
            }

            return View(tank);
        }

        // POST: Tanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tank = await _context.Tank.FindAsync(id);
            if (tank != null)
            {
                _context.Tank.Remove(tank);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TankExists(Guid id)
        {
            return _context.Tank.Any(e => e.Id == id);
        }
    }
}

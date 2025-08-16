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

        public TanksController(ApplicationDbContext Context)
        {
            _context = Context;
        }

        // GET: Tanks
        public async Task<IActionResult> Index(string SearchString, int Page = 1, int PageSize = 10)
        {
            if (Page < 1) Page = 1;

            var query = _context.Tank.AsQueryable();

            decimal? asDecimal = null;
            if (decimal.TryParse(SearchString, out var dec)) asDecimal = dec;

            DateTime? asDate = null;
            if (DateTime.TryParse(SearchString, out var dt)) asDate = dt;

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
                        .Skip((Page - 1) * PageSize)
                        .Take(PageSize)
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
            ViewBag.PageIndex = Page;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalCount = totalTanksCount;

            return View(tanks);
        }

        // GET: Tanks/Details/5
        public async Task<IActionResult> Details(Guid? Id)
        {
            if (Id == null) return NotFound();

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
                .FirstOrDefaultAsync(t => t.TankId == Id);

            if (tank == null) return NotFound();

            return View(tank);
        }

        // GET: Tanks/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tank Tank)
        {
            if (ModelState.IsValid)
            {
                Tank.Id = Guid.NewGuid();
                _context.Add(Tank);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(Tank);
        }

        // GET: Tanks/Edit/5
        public async Task<IActionResult> Edit(Guid? Id)
        {
            if (Id == null) return NotFound();
            
            var Tank = await _context.Tank
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
                    .FirstOrDefaultAsync(t => t.TankId == Id);

            if (Tank == null) return NotFound();
            
            return View(Tank);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, TankWaterQualityData TankWaterQualityModal)
        {
            if (!ModelState.IsValid) return View(TankWaterQualityModal);

            var Tank = await _context.Tank.Include(t => t.WaterQuality).FirstOrDefaultAsync(t => t.Id == Id);
            if (Tank == null) return NotFound();

            try
            {
                Tank.Shape = TankWaterQualityModal.Shape;
                Tank.Size = TankWaterQualityModal.Size;
                Tank.WaterQuality.PhLevel = TankWaterQualityModal.PhLevel;
                Tank.WaterQuality.Temperature = TankWaterQualityModal.Temperature;
                Tank.WaterQuality.AmmoniaLevel = TankWaterQualityModal.AmmoniaLevel;
                Tank.WaterQuality.WaterType = TankWaterQualityModal.WaterType;

                _context.Update(Tank);
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

            return RedirectToAction(nameof(Details), new { id = TankWaterQualityModal.TankId });
        }

        // GET: Tanks/Delete/5
        public async Task<IActionResult> Delete(Guid? Id)
        {
            if (Id == null) return NotFound();
            
            var Tank = await _context.Tank.FirstOrDefaultAsync(m => m.Id == Id);
            if (Tank == null) return NotFound();

            return View(Tank);
        }

        // POST: Tanks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid Id)
        {
            var tank = await _context.Tank.FindAsync(Id);
            if (tank != null) _context.Tank.Remove(tank);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TankExists(Guid Id)
        {
            return _context.Tank.Any(e => e.Id == Id);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualAquariumManager.Data;
using VirtualAquariumManager.Models;

namespace VirtualAquariumManager.Controllers
{
    public class TanksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TanksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tanks
        [Authorize]
        public async Task<IActionResult> Index(string SearchString, int page = 1, int pageSize = 10)
        {
            ViewBag.CurrentSearchString = SearchString;
            
            if (page < 1)
            {
                page = 1;
            }

            var query = _context.Tank.Include(t => t.WaterQuality).AsQueryable();

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
                            .ToListAsync();

            var totalTanksCount = await query.CountAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalTanksCount;

            return View(tanks);
        }

        // GET: Tanks/Details/5
        [Authorize]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tank = await _context.Tank
                .Include(t => t.WaterQuality)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tank == null)
            {
                return NotFound();
            }

            return View(tank);
        }

        // GET: Tanks/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tanks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tank = await _context.Tank
                .Include(t => t.WaterQuality)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tank == null)
            {
                return NotFound();
            }
            return View(tank);
        }

        // POST: Tanks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, Tank tank)
        {
            if (id != tank.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tank);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TankExists(tank.Id))
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
                    new { id = tank.Id }
                );
            }
            return View(tank);
        }

        // GET: Tanks/Delete/5
        [Authorize]
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
        [Authorize]
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

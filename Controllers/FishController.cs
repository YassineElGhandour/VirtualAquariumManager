using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualAquariumManager.Data;
using VirtualAquariumManager.Models;

namespace VirtualAquariumManager.Controllers
{
    [Authorize]
    public class FishController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FishController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fish?TankId=guid
        public async Task<IActionResult> Index(Guid? TankId, string SearchString, int page = 1, int pageSize = 10)
        {            
            page = page < 1 ? 1 : page;
            
            var query = _context.Fish.AsQueryable();

            if (TankId.HasValue)
            {
                query = query.Where(f => f.TankId == TankId);
            } else
            {
                return NotFound();
            }

            int? asInt = null;
            if (int.TryParse(SearchString, out var it))
            {
                asInt = it;
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

                query = query.Where(fish =>
                    EF.Functions.Like(fish.Name!, pattern)
                    || EF.Functions.Like(fish.SubSpecies!, pattern)
                    || (asInt.HasValue && fish.LifeSpan == asInt.Value)
                    || (asDate.HasValue && fish.ImportedDate.Date == asDate.Value.Date)
                );
            }

            var Tank = await _context.Tank.FirstOrDefaultAsync(t => t.Id == TankId);
            var fish = await query
                        .OrderBy(fish => fish.ImportedDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(fish => new Fish
                        {
                            Id = fish.Id,
                            TankId = fish.TankId,
                            Tank = Tank!,
                            Name = fish.Name!,
                            SubSpecies = fish.SubSpecies!,
                            LifeSpan = fish.LifeSpan,
                            ImportedDate = fish.ImportedDate
                        })
                        .ToListAsync();
            var totalfishCount = await query.CountAsync();

            // Prepare ViewBag for usage in Index.cshtml
            ViewBag.CurrentSearchString = SearchString;
            ViewBag.TankId = TankId;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalfishCount;

            return View(fish);
        }

        // GET: Fish/Details/5

        public async Task<IActionResult> Details(Guid? id, Guid? TankId)
        {
            if (id == null || TankId == null)
            {
                return NotFound();
            }

            var fish = await _context.Fish.FirstOrDefaultAsync(fish => fish.Id == id);

            if (fish == null)
            {
                return NotFound();
            }

            return View(fish);
        }

        // GET: Fish/Create
        public IActionResult Create(Guid? TankId)
        {
            Fish fish = new ()
            {
                TankId = TankId,
                ImportedDate = DateTime.Now
            };
            return View(fish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fish fish)
        {
            if (fish == null) return NotFound();
            if (!ModelState.IsValid) return View(fish);

            fish.Id = Guid.NewGuid();
            _context.Add(fish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { fish.TankId });
        }

        // GET: Fish/Edit/5
        public async Task<IActionResult> Edit(Guid? id, Guid? TankId)
        {
            if (id == null || TankId == null)
            {
                return NotFound();
            }

            var fish = await _context.Fish.FindAsync(id);

            if (fish == null)
            {
                return NotFound();
            }
            return View(fish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Fish fish)
        {
            if (fish == null || id != fish.Id) return NotFound();
            if (!ModelState.IsValid) return View(fish);

            try
            {
                _context.Update(fish);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FishExists(fish.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index), new { TankId = fish.TankId });
        }

        // GET: Fish/Delete/5
        public async Task<IActionResult> Delete(Guid? id, Guid? TankId)
        {
            if (id == null || TankId == null)
            {
                return NotFound();
            }

            var fish = await _context.Fish.FirstOrDefaultAsync(m => m.Id == id);
            if (fish == null)
            {
                return NotFound();
            }

            return View(fish);
        }

        // POST: Fish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fish = await _context.Fish.FindAsync(id);
            if (fish == null)
                return NotFound();

            var tankId = fish.TankId;

            _context.Fish.Remove(fish);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { TankId = tankId });
        }

        private bool FishExists(Guid id)
        {
            return _context.Fish.Any(e => e.Id == id);
        }
    }
}

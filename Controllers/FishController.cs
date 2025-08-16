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

        public FishController(ApplicationDbContext Context)
        {
            _context = Context;
        }

        // GET: Fish?TankId=guid
        public async Task<IActionResult> Index(Guid? TankId, string SearchString, int Page = 1, int PageSize = 10)
        {
            if (Page < 1) Page = 1;

            var Query = _context.Fish.AsQueryable();
            if (TankId.HasValue)
            {
                Query = Query.Where(f => f.TankId == TankId);
            } else
            {
                return NotFound();
            }

            int? AsInt = null;
            if (int.TryParse(SearchString, out var it)) AsInt = it;
            
            DateTime? AsDate = null;
            if (DateTime.TryParse(SearchString, out var dt)) AsDate = dt;

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                SearchString = SearchString.Trim();
                var Pattern = $"%{SearchString}%";

                Query = Query.Where(Fish =>
                    EF.Functions.Like(Fish.Name!, Pattern)
                    || EF.Functions.Like(Fish.SubSpecies!, Pattern)
                    || (AsInt.HasValue && Fish.LifeSpan == AsInt.Value)
                    || (AsDate.HasValue && Fish.ImportedDate.Date == AsDate.Value.Date)
                );
            }

            var Tank = await _context.Tank.FirstOrDefaultAsync(t => t.Id == TankId);
            var Fish = await Query
                        .OrderBy(Fish => Fish.ImportedDate)
                        .Skip((Page - 1) * PageSize)
                        .Take(PageSize)
                        .Select(Fish => new Fish
                        {
                            Id = Fish.Id,
                            TankId = Fish.TankId,
                            Tank = Tank!,
                            Name = Fish.Name!,
                            SubSpecies = Fish.SubSpecies!,
                            LifeSpan = Fish.LifeSpan,
                            ImportedDate = Fish.ImportedDate
                        })
                        .ToListAsync();
            var TotalfishCount = await Query.CountAsync();

            // Prepare ViewBag for usage in Index.cshtml
            ViewBag.CurrentSearchString = SearchString;
            ViewBag.TankId = TankId;
            ViewBag.PageIndex = Page;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalCount = TotalfishCount;

            return View(Fish);
        }

        // GET: Fish/Details/5
        public async Task<IActionResult> Details(Guid? Id)
        {
            if (Id == null) return NotFound();

            var Fish = await _context.Fish.FirstOrDefaultAsync(fish => fish.Id == Id);
            if (Fish == null) return NotFound();
            
            return View(Fish);
        }

        // GET: Fish/Create
        public IActionResult Create(Guid TankId)
        {
            if (TankId == Guid.Empty) return NotFound();
            
            Fish Fish = new ()
            {
                TankId = TankId,
                ImportedDate = DateTime.Now
            };
            return View(Fish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fish Fish)
        {
            if (Fish == null) return NotFound();
            if (!ModelState.IsValid) return View(Fish);

            Fish.Id = Guid.NewGuid();
            _context.Add(Fish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { Fish.TankId });
        }

        // GET: Fish/Edit/5
        public async Task<IActionResult> Edit(Guid? Id, Guid? TankId)
        {
            if (Id == null || TankId == null) return NotFound();
            
            var fish = await _context.Fish.FindAsync(Id);
            if (fish == null) return NotFound();
            
            return View(fish);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid Id, Fish Fish)
        {
            if (Fish == null || Id != Fish.Id) return NotFound();
            if (!ModelState.IsValid) return View(Fish);

            try
            {
                _context.Update(Fish);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FishExists(Fish.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index), new { Fish.TankId });
        }

        // GET: Fish/Delete/5
        public async Task<IActionResult> Delete(Guid? Id)
        {
            if (Id == null) return NotFound();
            
            var Fish = await _context.Fish.FirstOrDefaultAsync(Fish => Fish.Id == Id);
            if (Fish == null) return NotFound();

            return View(Fish);
        }

        // POST: Fish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid Id)
        {
            var Fish = await _context.Fish.FindAsync(Id);
            if (Fish == null) return NotFound();

            var tankId = Fish.TankId;
            _context.Fish.Remove(Fish);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { TankId = tankId });
        }

        private bool FishExists(Guid Id)
        {
            return _context.Fish.Any(e => e.Id == Id);
        }
    }
}

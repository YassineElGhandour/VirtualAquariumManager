using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualAquariumManager.Data;
using VirtualAquariumManager.Models;
using VirtualAquariumManager.ViewModels;

namespace VirtualAquariumManager.Controllers
{
    public class FishController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FishController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Fish
        [Authorize]
        public async Task<IActionResult> Index(string SearchString, int page = 1, int pageSize = 10)
        {
            ViewBag.CurrentSearchString = SearchString;

            if (page < 1)
            {
                page = 1;
            }

            var query = _context.Fish.AsQueryable();

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

            var fish = await query
                        .OrderBy(fish => fish.ImportedDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(fish => new Fish
                        {
                            Id = fish.Id,
                            Name = fish.Name!,
                            SubSpecies = fish.SubSpecies!,
                            LifeSpan = fish.LifeSpan,
                            ImportedDate = fish.ImportedDate
                        })
                        .ToListAsync();

            var totalfishCount = await query.CountAsync();

            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalfishCount;

            return View(fish);
        }

        // GET: Fish/Details/5
        [Authorize]

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
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
        [Authorize]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Fish/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Fish fish)
        {
            if (ModelState.IsValid)
            {
                fish.Id = Guid.NewGuid();
                _context.Add(fish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fish);
        }

        // GET: Fish/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
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

        // POST: Fish/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Species,LifeSpan")] Fish fish)
        {
            if (id != fish.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FishExists(fish.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fish);
        }

        // GET: Fish/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fish = await _context.Fish
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fish == null)
            {
                return NotFound();
            }

            return View(fish);
        }

        // POST: Fish/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fish = await _context.Fish.FindAsync(id);
            if (fish != null)
            {
                _context.Fish.Remove(fish);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FishExists(Guid id)
        {
            return _context.Fish.Any(e => e.Id == id);
        }
    }
}

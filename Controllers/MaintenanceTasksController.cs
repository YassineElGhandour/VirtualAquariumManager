using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VirtualAquariumManager.Data;
using VirtualAquariumManager.Models;

namespace VirtualAquariumManager.Controllers
{
    public class MaintenanceTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MaintenanceTasks?TankId=guid
        public async Task<IActionResult> Index(Guid? TankId, string SearchString, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;

            var query = _context.MaintenanceTask.AsQueryable();

            if (TankId.HasValue)
            {
                query = query.Where(task => task.TankId == TankId);
            }
            else
            {
                return NotFound();
            }

            bool? asBool = null;
            if (bool.TryParse(SearchString, out var bol))
            {
                asBool = bol;
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

                query = query.Where(task =>
                    (asDate.HasValue && task.DueDate.Date == asDate.Value.Date)
                    || (asDate.HasValue && task.PerformedOn.Date == asDate.Value.Date)
                    || (asBool.HasValue && task.IsCompleted == asBool.Value)
                );
            }

            var Tank = await _context.Tank.FirstOrDefaultAsync(t => t.Id == TankId);
            var MaintenanceTask = await query
                        .OrderBy(task => task.DueDate)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(task => new MaintenanceTask
                        {
                            TankId = task.TankId,
                            Tank = Tank!,
                            DueDate = task.DueDate!,
                            PerformedOn = task.PerformedOn!,
                            IsCompleted = task.IsCompleted,
                            Type = task.Type,
                        })
                        .ToListAsync();
            var TotalMaintenanceTasksCount = await query.CountAsync();

            // Prepare ViewBag for usage in Index.cshtml
            ViewBag.CurrentSearchString = SearchString;
            ViewBag.TankId = TankId;
            ViewBag.PageIndex = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = TotalMaintenanceTasksCount;

            return View(MaintenanceTask);
        }

        // GET: MaintenanceTasks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceTask = await _context.MaintenanceTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceTask == null)
            {
                return NotFound();
            }

            return View(maintenanceTask);
        }

        // GET: MaintenanceTasks/Create
        public async Task<IActionResult> Create(Guid TankId)
        {
            if (TankId == Guid.Empty)
            {
                return NotFound();
            }

            var Tank = await _context.Tank.FirstOrDefaultAsync(t => t.Id == TankId);
            if (Tank == null)
            {
                return NotFound();
            }

            MaintenanceTask MaintenanceTask = new()
            {
                TankId = TankId,
                Tank = Tank,
                IsCompleted = false,
                Type = MaintenanceType.WaterChange
            };
            return View(MaintenanceTask);
        }

        // POST: MaintenanceTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceTask MaintenanceTask)
        {
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            MaintenanceTask.Id = Guid.NewGuid();
            _context.Add(MaintenanceTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { MaintenanceTask.TankId });
        }

        // GET: MaintenanceTasks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceTask = await _context.MaintenanceTask.FindAsync(id);
            if (maintenanceTask == null)
            {
                return NotFound();
            }
            return View(maintenanceTask);
        }

        // POST: MaintenanceTasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MaintenanceTask maintenanceTask)
        {
            if (id != maintenanceTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceTaskExists(maintenanceTask.Id))
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
            return View(maintenanceTask);
        }

        // GET: MaintenanceTasks/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceTask = await _context.MaintenanceTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceTask == null)
            {
                return NotFound();
            }

            return View(maintenanceTask);
        }

        // POST: MaintenanceTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var maintenanceTask = await _context.MaintenanceTask.FindAsync(id);
            if (maintenanceTask != null)
            {
                _context.MaintenanceTask.Remove(maintenanceTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceTaskExists(Guid id)
        {
            return _context.MaintenanceTask.Any(e => e.Id == id);
        }
    }
}

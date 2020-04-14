using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Core.Models;
using TicketManagementSystem.Data;

namespace TicketManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public static readonly string BITOREQNAME = "Bitoreq";

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.Developer1User).Include(p => p.Developer2User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.Developer1User)
                .Include(p => p.Developer2User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult AddProject()
        {
            IEnumerable<SelectListItem> developers = GetDevelopersList();
            
            ViewData["CompanyName"] = new SelectList(_context.Companies.Where(c => c.CompanyName != BITOREQNAME), "Id", "CompanyName");
            ViewData["Developer1"] = developers;
            ViewData["Developer2"] = developers;
            return View();
        }

        private IEnumerable<SelectListItem> GetDevelopersList()
        {
            var developerRole = _context.Roles.Where(r => r.Name == "Developer").FirstOrDefault().Id;
            var developersId = _context.UserRoles.Where(ur => ur.RoleId == developerRole).Select(u => u.UserId).ToList();

            IEnumerable<SelectListItem> developers = _context.Users.Where(u => developersId.Contains(u.Id)).Select(i => new SelectListItem()
            {
                Text = i.UserName,
                Value = i.Id
            });
            
            return developers;
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProject([Bind("Id,CompanyId,Name,Developer1,Developer2,ReleaseDate,LastUpdate,Description,SystemsUsed")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<SelectListItem> developers = GetDevelopersList();

            ViewData["CompanyName"] = new SelectList(_context.Companies, "Id", "CompanyName", project.CompanyId);
            ViewData["Developer1"] = developers;
            ViewData["Developer2"] = developers;
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(c => c.Company).Where(p => p.Id == id).FirstOrDefaultAsync();
            
            if (project == null)
            {
                return NotFound();
            }

            var tickets = _context.Tickets.Where(t => t.ProjectId == id).Count();

            if (tickets == 0)
            {
                ViewData["company"] = "show";
            }
            else
            {
                ViewData["company"] = "hide";
            }


            IEnumerable<SelectListItem> developers = GetDevelopersList();

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyAbbr", project.CompanyId);
            ViewData["Developer1"] = developers;
            ViewData["Developer2"] = developers;

            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Developer1,Developer2,ReleaseDate,LastUpdate,Description,SystemsUsed")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
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

            IEnumerable<SelectListItem> developers = GetDevelopersList();

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyAbbr", project.CompanyId);
            ViewData["Developer1"] = developers;
            ViewData["Developer2"] = developers;
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.Developer1User)
                .Include(p => p.Developer2User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var anytickets= _context.Tickets.Where(t=>t.ProjectId==id).Count();
            

            if (anytickets == 0)
            {
                ViewData["formhide"] = "Show";
            }
            else
            {
                ViewData["formhide"] = "Hide";
            }




            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CheckLastUpdate(DateTime ReleaseDate, DateTime LastUpdate)
        {
            if (LastUpdate < ReleaseDate)
            {
                return Json($"{LastUpdate} är inte giltig. Borde vara större än släppdatum.");
            }

            return Json(true);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}

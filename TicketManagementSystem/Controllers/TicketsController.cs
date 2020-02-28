using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TicketManagementSystem.Core.Models;
using TicketManagementSystem.Core.ViewModels;
using TicketManagementSystem.Data;

namespace TicketManagementSystem.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        // GET: Tickets
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            IQueryable<Ticket> applicationdbcontext = _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project);

            applicationdbcontext = SortList(sortOrder, applicationdbcontext);

            return View(await applicationdbcontext.ToListAsync());
        }

        public async Task<IActionResult> Filter(string title, int? status, int? priority)
        {
            var model = await _context.Tickets.ToListAsync();
            model = string.IsNullOrWhiteSpace(title) ?
                model :
                model.Where(p => p.Title.ToLower().Contains(title.ToLower())).ToList();

            model = status == null ?
              model :
              model.Where(m => m.Status == (Status)status).ToList();

            model = priority == null ?
                model :
                model.Where(m => m.CustomerPriority == (Priority)priority).ToList();

            return View(nameof(Index), model);
        }

        private IQueryable<Ticket> SortList(string sortOrder, IQueryable<Ticket> ticket)
        {
            ViewBag.RefNoSortParm = String.IsNullOrEmpty(sortOrder) ? "RefNo_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewBag.ProjectIdSortParm = sortOrder == "ProjectId" ? "ProjectId_desc" : "ProjectId";
            ViewBag.CustomerPrioritySortParm = sortOrder == "CustomerPriority" ? "CustomerPriority_desc" : "CustomerPriority";
            ViewBag.DueDateSortParm = sortOrder == "DueDate" ? "DueDate_desc" : "DueDate";

            switch (sortOrder)
            {
                case "RefNo_desc":
                    ticket = ticket.OrderByDescending(s => s.RefNo);
                    break;

                case "Title_desc":
                    ticket = ticket.OrderByDescending(s => s.Title);
                    break;

                case "Status_desc":
                    ticket = ticket.OrderByDescending(s => s.Status);
                    break;

                case "ProjectId_desc":

                    ticket = ticket.OrderByDescending(s => s.ProjectId);
                    break;

                case "CustomerPriority_desc":

                    ticket = ticket.OrderByDescending(s => s.CustomerPriority);
                    break;

                case "DueDate_desc":

                    ticket = ticket.OrderByDescending(s => s.DueDate);
                    break;

                default:

                    ticket = ticket.OrderBy(s => s.RefNo);
                    break;

            }

            return ticket;
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.AssignedUser)
                .Include(t => t.CreatedUser)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/AddTicket
        public IActionResult AddTicket()
        {
            ViewData["AssignedTo"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["CreatedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }

        // POST: Tickets/AddTicket
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicket([Bind("Id,Title,Problem,CreatedBy,CreatedDate,AssignedTo,HoursSpent,Status,ProjectId,CustomerPriority,RealPriority,DueDate,ClosedDate,LastUpdated,ResponseType,ResponseDesc")] Ticket ticket)
        {
            //Getting LoggedInUser's ComapnyId & then CompanyAbbr & Last RefNo of that company
            ApplicationUser loggedInUser = await userManager.GetUserAsync(User);
            var companyAbbr = _context.Companies.Find(loggedInUser.CompanyId).CompanyAbbr;
            //var companyLastRefNo = _context.Tickets.LastOrDefault(t => t.RefNo.Contains(companyAbbr)).RefNo;
            var companyLastRefNo = _context.Tickets.Where(t => t.RefNo.Contains(companyAbbr)).ToList().LastOrDefault().RefNo;

            //Increasin that last RefNo by 1 and assigning it to the newly added ticket
            ticket.RefNo = Regex.Replace(companyLastRefNo, "\\d+",
                m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));

            ticket.Status = Status.Draft;
            ticket.DueDate = DateTime.Now;



            //var userId = userManager.GetUserAsync(User);
            //var user = userManager.FindByIdAsync(ticket.CreatedBy);

            //var companyLastRefNo = await _context.Tickets
            //    .LastOrDefaultAsync(m => m.RefNo == id)
            //var companyLastRefNo = User.
            //var companyLastRefNo =  _context.Users
            //    .Where(u => u.Id == ticket.CreatedBy);
            //    .Include(a => a.CreatedUser)
            //    .ThenInclude(m => m.Company)
            //    .LastOrDefaultAsync(a => a.RefNo.Substring(0,5) == );

            //_context.Tickets.(ticket.CreatedUser.CompanyId)
            //var newString = Regex.Replace(x, "\\d+",
            //    m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
            //ticket.RefNo
            if (ModelState.IsValid)
            {                
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignedTo"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.AssignedTo);
            ViewData["CreatedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["AssignedTo"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.AssignedTo);
            ViewData["CreatedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,RefNo,Title,Problem,CreatedBy,CreatedDate,AssignedTo,HoursSpent,Status,ProjectId,CustomerPriority,RealPriority,DueDate,ClosedDate,LastUpdated,ResponseType,ResponseDesc")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
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
            ViewData["AssignedTo"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.AssignedTo);
            ViewData["CreatedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.CreatedBy);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.AssignedUser)
                .Include(t => t.CreatedUser)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(long id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}

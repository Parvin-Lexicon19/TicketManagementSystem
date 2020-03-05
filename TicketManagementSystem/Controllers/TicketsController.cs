using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        static ApplicationUser loggedInUser;
        List<SelectListItem> selectListItems;
        private readonly IEmailSender _emailSender;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            this.userManager = userManager;
            selectListItems = new List<SelectListItem>();
            _emailSender = emailSender;
        }

        // GET: Tickets
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder)
        {
            //var applicationdbcontext = _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project);
            // return View(await applicationdbcontext.ToListAsync());

            //var loggedInUser = await userManager.GetUserAsync(User);
            //var UserRole = await userManager.GetRolesAsync(loggedInUser);


            List<TicketIndexViewModel> model;

            if (User.IsInRole("Admin") || User.IsInRole("Developer"))
            {
                model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                    .Where(s => s.Status != Status.Draft)
                    .Select(s => new TicketIndexViewModel
                    {
                        Id = s.Id,
                        RefNo = s.RefNo,
                        Title = s.Title,
                        Status = s.Status,
                        ProjectName = s.Project.Name,
                        CustomerPriority = s.CustomerPriority,
                        RealPriority = s.RealPriority,
                        DueDate = s.DueDate,
                        UserEmail = s.AssignedUser.Email
                    })
                    .ToListAsync();

            }
            else
            {
                model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                        .Select(s => new TicketIndexViewModel
                        {
                            Id = s.Id,
                            RefNo = s.RefNo,
                            Title = s.Title,
                            Status = s.Status,
                            ProjectName = s.Project.Name,
                            CustomerPriority = s.CustomerPriority,
                            RealPriority = s.RealPriority,
                            DueDate = s.DueDate,
                            UserEmail = s.AssignedUser.Email
                        })
                        .ToListAsync();
            }

            model = SortList(sortOrder, model);
            return View(model);
        }

        //Filter by Title, Status and Priority
        public async Task<IActionResult> Filter(string title, int? StatusSearch, int? customerPriority, int? adminPriority)
        {
            List<TicketIndexViewModel> model;

            if (User.IsInRole("Admin") || User.IsInRole("Developer"))
            {
                model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                    .Where(s => s.Status != Status.Draft)
                    .Select(s => new TicketIndexViewModel
                    {
                        Id = s.Id,
                        RefNo = s.RefNo,
                        Title = s.Title,
                        Status = s.Status,
                        ProjectName = s.Project.Name,
                        CustomerPriority = s.CustomerPriority,
                        RealPriority = s.RealPriority,
                        DueDate = s.DueDate,
                        UserEmail = s.AssignedUser.Email
                    })
                    .ToListAsync();

            }
            else
            {
                model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                        .Select(s => new TicketIndexViewModel
                        {
                            Id = s.Id,
                            RefNo = s.RefNo,
                            Title = s.Title,
                            Status = s.Status,
                            ProjectName = s.Project.Name,
                            CustomerPriority = s.CustomerPriority,
                            RealPriority = s.RealPriority,
                            DueDate = s.DueDate,
                            UserEmail = s.AssignedUser.Email
                        })
                        .ToListAsync();
            }

            model = string.IsNullOrWhiteSpace(title) ?
                model :
                model.Where(p => p.Title.ToLower().Contains(title.ToLower())).ToList();

            model = StatusSearch == null ?
              model :
              model.Where(m => m.Status == (Status)StatusSearch).ToList();

            model = customerPriority == null ?
                model :
                model.Where(m => m.CustomerPriority == (Priority)customerPriority).ToList();

            model = adminPriority == null ?
                model :
                model.Where(m => m.RealPriority == (Priority)adminPriority).ToList();

            return View(nameof(Index), model);
        }

        //Sort by Attributes
        private List<TicketIndexViewModel> SortList(string sortOrder, List<TicketIndexViewModel> ticket)
        {
            ViewBag.RefNoSortParm = String.IsNullOrEmpty(sortOrder) ? "RefNo_desc" : "";
            ViewBag.TitleSortParm = sortOrder == "Title" ? "Title_desc" : "Title";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "Status_desc" : "Status";
            ViewBag.ProjectNameSortParm = sortOrder == "ProjectName" ? "ProjectName_desc" : "ProjectName";
            ViewBag.CustomerPrioritySortParm = sortOrder == "CustomerPriority" ? "CustomerPriority_desc" : "CustomerPriority";
            ViewBag.RealrPrioritySortParm = sortOrder == "RealrPriority" ? "RealrPriority_desc" : "RealrPriority";
            ViewBag.DueDateSortParm = sortOrder == "DueDate" ? "DueDate_desc" : "DueDate";
            ViewBag.AssignedToSortParm = sortOrder == "AssignedTo" ? "AssignedTo_desc" : "AssignedTo";

            switch (sortOrder)
            {
                case "RefNo_desc":
                    ticket = ticket.OrderByDescending(s => s.RefNo).ToList();
                    break;

                case "Title_desc":
                    ticket = ticket.OrderByDescending(s => s.Title).ToList();
                    break;

                case "Status_desc":
                    ticket = ticket.OrderByDescending(s => s.Status).ToList();
                    break;

                case "ProjectName_desc":
                    ticket = ticket.OrderByDescending(s => s.ProjectName).ToList();
                    break;

                case "CustomerPriority_desc":
                    ticket = ticket.OrderByDescending(s => s.CustomerPriority).ToList();
                    break;

                case "RealrPriority_desc":
                    ticket = ticket.OrderByDescending(s => s.RealPriority).ToList();
                    break;

                case "DueDate_desc":
                    ticket = ticket.OrderByDescending(s => s.DueDate).ToList();
                    break;

                case "AssignedTo_desc":
                    ticket = ticket.OrderByDescending(s => s.UserEmail).ToList();
                    break;

                default:
                    ticket = ticket.OrderBy(s => s.RefNo).ToList();
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
        public async Task<IActionResult> AddTicketAsync()
        {
            ViewData["AssignedTo"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            ViewData["CreatedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id");
            //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");

            loggedInUser = await userManager.GetUserAsync(User);
            
            var loggedInUserProjects = _context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId);
            foreach (var project in loggedInUserProjects)
            {
                var selectItem = new SelectListItem
                {
                    Text = project.Name,
                    Value = project.Id.ToString()
                };
                selectListItems.Add(selectItem);
            }
            ViewData["ProjectId"] = selectListItems;
            return View();
        }

        // POST: Tickets/AddTicket
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicket([Bind("Id,Title,Problem,CreatedBy,CreatedDate,AssignedTo,HoursSpent,Status,ProjectId,CustomerPriority,RealPriority,DueDate,ClosedDate,LastUpdated,ResponseType,ResponseDesc")] Ticket ticket, string submit)
        {
            //Getting LoggedInUser's ComapnyId & then CompanyAbbr & Last RefNo of that company
            var companyAbbr = _context.Companies.Find(loggedInUser.CompanyId).CompanyAbbr;
            //var companyLastRefNo = _context.Tickets.LastOrDefault(t => t.RefNo.Contains(companyAbbr)).RefNo;
            bool companyHasTicket = _context.Tickets.Any(t => t.RefNo.Contains(companyAbbr));

            //if the company has no tickets, the last RefNo. is se to "00000", otherwise it continues from the last RefNo. for that company
            string companyLastRefNo = companyHasTicket == true? 
                _context.Tickets.Where(t => t.RefNo.Contains(companyAbbr)).ToList().LastOrDefault().RefNo
                : companyLastRefNo = companyAbbr + "00000";
                
            //Increasing that last RefNo by 1 and assigning it to the newly added ticket
            ticket.RefNo = Regex.Replace(companyLastRefNo, "\\d+",
                m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));

            ticket.DueDate = DateTime.Now;

            switch (submit)
            {
                case "Submit":
                    ticket.Status = Status.Submitted;                    
                    break;

                case "Save as Draft":
                    ticket.Status = Status.Draft;                    
                    break;

                default:
                    throw new Exception();
            }


            if (ModelState.IsValid)
            {                
                _context.Add(ticket);
                await _context.SaveChangesAsync();

                if(ticket.Status.Equals(Status.Submitted))
                {
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(
                    "admin@bitoreq.se",
                    "A New Ticket Submitted",
                    $"A new ticket submitted by {loggedInUser.Email}. " +
                    $"See the ticket here: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> Details.");
                    return RedirectToAction(nameof(EmailSent));
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["AssignedTo"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.AssignedTo);
            ViewData["CreatedBy"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", ticket.CreatedBy);
            ViewData["ProjectId"] = selectListItems;

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

        public ActionResult EmailSent()
        {
            return View();
        }
    }
}

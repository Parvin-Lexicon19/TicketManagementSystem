using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TicketManagementSystem.Core.Models;
using TicketManagementSystem.Core.ViewModels;
using TicketManagementSystem.Data;
using System.Diagnostics;
using TicketManagementSystem.Models;

namespace TicketManagementSystem.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext _context;
        private static ApplicationUser loggedInUser;
        List<ApplicationUser> ticketProjectDevelopers;
        private readonly IEmailSender _emailSender;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            this.userManager = userManager;
            //selectListItems = new List<SelectListItem>();
            _emailSender = emailSender;
        }

        // GET: Tickets
        [Authorize]
        public async Task<IActionResult> Index(string sortOrder, List<TicketIndexViewModel> model)
        {
            //var applicationdbcontext = _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project);
            // return View(await applicationdbcontext.ToListAsync());

            var loggedInUser = await userManager.GetUserAsync(User);

            // List all the result 
            if (User.IsInRole("Admin") || User.IsInRole("Developer"))
            {
                model = await TicketViewModelAdmin(model);
            }
            else
            {
                model = await TicketViewModelCustomer(model, loggedInUser);
            }

            // Sort by attributes in the list
            //model = SortList(sortOrder, model);
            return View(model);
        }

        // Index for Customer Company Tickets  
        public async Task<IActionResult> IndexCompanyTickets(string sortOrder, List<TicketIndexViewModel> model, string title, int? statusSearch, int? customerPriority, int? priorities )
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            // List all the result by Company
            model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                        .Where(u => u.CreatedUser.CompanyId == loggedInUser.CompanyId)
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


            // Sort by attributes in the list
            // model = SortList(sortOrder, model);
            return View(model);
        }

        // Customer tickets ViewModel and List all the result by UserId
        private async Task<List<TicketIndexViewModel>> TicketViewModelCustomer(List<TicketIndexViewModel> model, ApplicationUser loggedInUser)
        {
            model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                    .Where(u => u.CreatedBy == loggedInUser.Id)
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
            return model;
        }

        // Admin tickets ViewModel and List all the result and not show Draft status and Closed status
        private async Task<List<TicketIndexViewModel>> TicketViewModelAdmin(List<TicketIndexViewModel> model)
        {
            model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                .Where(s => s.Status != Status.Draft && s.Status != Status.Closed)
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
            return model;
        }

        //Filter by Title, Status and Priority
        public async Task<IActionResult> Filter(string title, int? statusSearch, int? customerPriority, int? adminPriority, int? priorities, List<TicketIndexViewModel> model, List<TicketIndexViewModel> model2, string sortOrder, string currentFilter)
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            // List all the result 
            if (User.IsInRole("Admin") || User.IsInRole("Developer"))
            {
                model = await TicketViewModelAdmin(model);
            }
            else
            {
                model = await TicketViewModelCustomer(model, loggedInUser);
            }

            // List all the result and not show Draft Status 
            model2 = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
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


            // Search by Title
            model = string.IsNullOrWhiteSpace(title) ?
                model :
                model.Where(p => p.Title.ToLower().Contains(title.ToLower())).ToList();

            // Search by Status
            if (User.IsInRole("Admin") || User.IsInRole("Developer"))
            {              
              model = statusSearch == null ?
              model :
              model2.Where(m => m.Status == (Status)statusSearch).ToList();
            }
            else
            {
             model = statusSearch == null ?
             model :
             model.Where(m => m.Status == (Status)statusSearch).ToList();
            }

            // Search by Customer Priority Drowpdown
            model = customerPriority == null ?
                model :
                model.Where(m => m.CustomerPriority == (Priority)customerPriority).ToList();

            // Search by Real Priority Drowpdown
            model = adminPriority == null ?
                model :
                model.Where(m => m.RealPriority == (Priority)adminPriority).ToList();

            // Search by Match or Not-Match Priority Drowpdown
            if (priorities.GetValueOrDefault() == 1)
            {
                model = model.Where(m => m.RealPriority == m.CustomerPriority).ToList();
            }
            if (priorities.GetValueOrDefault() == 2)
            {
                model = model.Where(m => m.RealPriority != m.CustomerPriority).ToList();
            }

            //model = SortList(sortOrder, model);  
            return View(nameof(Index), model);
        }

        //Filter For company Tickets by Title, Status and Priorities
        public async Task<IActionResult> FilterForCompanyTickets(string title, int? statusSearch, int? customerPriority, int? priorities, List<TicketIndexViewModel> model, string sortOrder)
        {
            var loggedInUser = await userManager.GetUserAsync(User);
           
            // List all the result by CompanyId 
            model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                        .Where(u => u.CreatedUser.CompanyId == loggedInUser.CompanyId)
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

            // Search by Title
             model = string.IsNullOrWhiteSpace(title) ?
             model :
             model.Where(p => p.Title.ToLower().Contains(title.ToLower())).ToList();

            // Search by Status
            model = statusSearch == null ?
             model :
             model.Where(m => m.Status == (Status)statusSearch).ToList();

            // Search by Customer Priority Drowpdown
             model = customerPriority == null ?
             model :
             model.Where(m => m.CustomerPriority == (Priority)customerPriority).ToList();

             //model = SortList(sortOrder, model);
             return View(nameof(IndexCompanyTickets), model);

        }

        // Sort by attributes in the list
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

                case "RefNo":
                    ticket = ticket.OrderBy(s => s.RefNo).ToList();
                    break;

                case "Title_desc":
                    ticket = ticket.OrderByDescending(s => s.Title).ToList();
                    break;

                case "Title":
                    ticket = ticket.OrderBy(s => s.Title).ToList();
                    break;

                case "Status_desc":
                    ticket = ticket.OrderByDescending(s => s.Status).ToList();
                    break;

                case "Status":
                    ticket = ticket.OrderBy(s => s.Status).ToList();
                    break;

                case "ProjectName_desc":
                    ticket = ticket.OrderByDescending(s => s.ProjectName).ToList();
                    break;

                case "ProjectName":
                    ticket = ticket.OrderBy(s => s.ProjectName).ToList();
                    break;

                case "CustomerPriority_desc":
                    ticket = ticket.OrderByDescending(s => s.CustomerPriority).ToList();
                    break;

                case "CustomerPriority":
                    ticket = ticket.OrderBy(s => s.CustomerPriority).ToList();
                    break;

                case "RealrPriority_desc":
                    ticket = ticket.OrderByDescending(s => s.RealPriority).ToList();
                    break;

                case "RealrPriority":
                    ticket = ticket.OrderBy(s => s.RealPriority).ToList();
                    break;

                case "DueDate_desc":
                    ticket = ticket.OrderByDescending(s => s.DueDate).ToList();
                    break;

                case "DueDate":
                    ticket = ticket.OrderBy(s => s.DueDate).ToList();
                    break;

                case "AssignedTo_desc":
                    ticket = ticket.OrderByDescending(s => s.UserEmail).ToList();
                    break;

                case "AssignedTo":
                    ticket = ticket.OrderBy(s => s.UserEmail).ToList();
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
            ticket.Comments = await _context.Comments.Where(m => m.TicketId == id).Include(c => c.ApplicationUser).ToListAsync();
            ticket.Documents = await _context.Documents.Where(d => d.TicketId == id).ToListAsync();

            var model = new TicketDetailsViewModel
            {
                Ticket = ticket,
                Documents = ticket.Documents,
                Comment = new Comment
                {
                    TicketId = ticket.Id,
                }
            };
            //Pass the LoggedInUser when closing the Ticket.
            loggedInUser = await userManager.GetUserAsync(User);
            TempData["loggedInUser"] = loggedInUser.Email;

            return View(model);
        }

        // GET: Tickets/AddTicket
        public async Task<IActionResult> AddTicketAsync()
        {
            var model = new AddTicketViewModel();

            if (User.IsInRole("Developer") || User.IsInRole("Admin"))
            {
                var customers = await userManager.GetUsersInRoleAsync("Customer");
                var confirmedCustomers = customers.Where(a => a.EmailConfirmed == true);
                ViewData["CreatedBy"] = new SelectList(confirmedCustomers, "Id", "Email").OrderBy(m => m.Text);   
                
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            }

            else if (User.IsInRole("Customer"))
            {
                loggedInUser = await userManager.GetUserAsync(User);
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");
            }

            return View(model);
        }

        // POST: Tickets/AddTicket
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicket(AddTicketViewModel model, string submit)
        {
            if (User.IsInRole("Developer") || User.IsInRole("Admin"))
                loggedInUser = _context.ApplicationUsers.FirstOrDefault(a => a.Id == model.Ticket.CreatedBy);
            else if (User.IsInRole("Customer"))
                model.Ticket.CreatedBy = loggedInUser.Id;

            /*Getting LoggedInUser's ComapnyId & then CompanyAbbr & Last RefNo of that company*/
            Company loggedInUserCompany = _context.Companies.Find(loggedInUser.CompanyId);
            var companyAbbr = loggedInUserCompany.CompanyAbbr;
            bool companyHasTicket = _context.Tickets.Any(t => t.RefNo.Contains(companyAbbr));

            /*if the company has no tickets, the last RefNo. is se to "00000", otherwise it continues from the last RefNo. for that company*/
            string companyLastRefNo = companyHasTicket == true? 
                _context.Tickets.Where(t => t.RefNo.Contains(companyAbbr)).ToList().LastOrDefault().RefNo
                : companyLastRefNo = companyAbbr + "00000";

            /*Increasing that last RefNo by 1 and assigning it to the newly added ticket*/
            model.Ticket.RefNo = Regex.Replace(companyLastRefNo, "\\d+",
                m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));
            
            model.Ticket.CreatedDate = DateTime.Now;
            model.Ticket.RealPriority = model.Ticket.CustomerPriority;

            switch (model.Ticket.CustomerPriority)
            {
                case Priority.A_2days:
                    model.Ticket.DueDate = DateTime.Now.AddDays(2);
                    break;
                case Priority.B_5days:
                    model.Ticket.DueDate = DateTime.Now.AddDays(5);
                    break;
                case Priority.C_9days:
                    model.Ticket.DueDate = DateTime.Now.AddDays(9);
                    break;

                default:
                    throw new Exception();
            }            

            switch (submit)
            {
                case "Submit":
                    model.Ticket.Status = Status.Submitted;
                    var ticketProject = await _context.Projects.FirstOrDefaultAsync(g => g.Id == model.Ticket.ProjectId);
                    ticketProjectDevelopers = new List<ApplicationUser>();
                    if (ticketProject.Developer1 != null)
                    {
                        ticketProjectDevelopers.Add(await userManager.FindByIdAsync(ticketProject.Developer1));
                        model.Ticket.AssignedTo = ticketProject.Developer1;
                    }
                    if (ticketProject.Developer2 != null)
                        ticketProjectDevelopers.Add(await userManager.FindByIdAsync(ticketProject.Developer2));
                    break;

                case "Save as Draft":
                    model.Ticket.Status = Status.Draft;                    
                    break;

                default:
                    throw new Exception();
            }

            if (ModelState.IsValid)
            {
                _context.Add(model.Ticket);
                await _context.SaveChangesAsync();

                if (model.File != null)
                    Fileupload(model.File, model.Ticket.Id, model.Ticket.CreatedBy, model.Ticket.RefNo);

                /*Ticket assigns to developer1, but emails sent to both developer1 and developer2*/
                if (model.Ticket.Status.Equals(Status.Submitted))
                {
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = model.Ticket.Id }, protocol: Request.Scheme);

                    foreach (var developer in ticketProjectDevelopers)
                    {
                        await _emailSender.SendEmailAsync(
                          developer.Email,
                          "A New Ticket Submitted",
                          $"Hello dear {developer.FirstName}," +
                          $"<br/><br/>A new ticket submitted by {loggedInUser.Email} from <b>{loggedInUserCompany.CompanyName}</b> Company. " +
                          $"<br/>Please see the ticket here: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> Ticket Details</a>." +
                          $"<br/><br/>Thank you,<br/>Bitoreq Admin");
                    }

                    return RedirectToAction(nameof(EmailSent));
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
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
            ticket.Documents = await _context.Documents.Where(d => d.TicketId == id).ToListAsync();

            var model = new TicketDetailsViewModel
            {
                Ticket = ticket,
                Documents = ticket.Documents,
            };

            loggedInUser = await userManager.GetUserAsync(User);
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");

            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, TicketDetailsViewModel model, string submit)
        {
            model.Ticket.RealPriority = model.Ticket.CustomerPriority;
            switch (model.Ticket.CustomerPriority)
            {
                case Priority.A_2days:
                    model.Ticket.DueDate = DateTime.Now.AddDays(2);
                    break;
                case Priority.B_5days:
                    model.Ticket.DueDate = DateTime.Now.AddDays(5);
                    break;
                case Priority.C_9days:
                    model.Ticket.DueDate = DateTime.Now.AddDays(9);
                    break;

                default:
                    throw new Exception();
            }
            switch (submit)
            {
                case "Submit":
                    model.Ticket.Status = Status.Submitted;
                    model.Ticket.CreatedDate = DateTime.Now;
                    var ticketProject = await _context.Projects.FirstOrDefaultAsync(g => g.Id == model.Ticket.ProjectId);
                    ticketProjectDevelopers = new List<ApplicationUser>();
                    if (ticketProject.Developer1 != null)
                    {
                        ticketProjectDevelopers.Add(await userManager.FindByIdAsync(ticketProject.Developer1));
                        model.Ticket.AssignedTo = ticketProject.Developer1;
                    }
                    if (ticketProject.Developer2 != null)
                        ticketProjectDevelopers.Add(await userManager.FindByIdAsync(ticketProject.Developer2));
                    break;

                case "Save as Draft":
                    break;

                default:
                    throw new Exception();
            }
            if (id != model.Ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.Ticket);
                    await _context.SaveChangesAsync();

                    if (model.File != null)
                        Fileupload(model.File, model.Ticket.Id, model.Ticket.CreatedBy, model.Ticket.RefNo);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(model.Ticket.Id))
                    {
                        return NotFound();
                    }
                }

                /*Ticket assigns to developer1, but emails sent to both developer1 and developer2*/
                if (model.Ticket.Status.Equals(Status.Submitted))
                {
                    Company loggedInUserCompany = _context.Companies.Find(loggedInUser.CompanyId);
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = model.Ticket.Id }, protocol: Request.Scheme);

                    foreach (var developer in ticketProjectDevelopers)
                    {
                        await _emailSender.SendEmailAsync(
                          developer.Email,
                          "A New Ticket Submitted",
                          $"Hello dear {developer.FirstName}," +
                          $"<br/><br/>A new ticket submitted by {loggedInUser.Email} from <b>{loggedInUserCompany.CompanyName}</b> Company. " +
                          $"<br/>Please see the ticket here: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> Ticket Details</a>." +
                          $"<br/><br/>Thank you,<br/>Bitoreq Admin");
                    }

                    return RedirectToAction(nameof(EmailSent));
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
        // Edit Ticket Through Detail Screen.
        [HttpPost]
        public string SaveResponse(long id, double HoursSpent, Status Status, string RespDesc, ResponseType RespType, Priority RelPriority)
        {
            var newticket =  _context.Tickets.Find(id);

            string loggedInUser = (string)TempData["loggedInUser"];
           var createdUser = _context.ApplicationUsers.FirstOrDefault(a => a.Id == newticket.CreatedBy);
            

            TempData.Keep();

            if (newticket == null)
            {
                return "The ticket not found";
            }

            // Update only the changed value to database.

            //Status
            if (newticket.Status != (Status)(Status))
            {
                newticket.Status = (Status)(Status);
            }

            //Response Type
            if (newticket.ResponseType != (ResponseType)(RespType))
            {
                newticket.ResponseType = (ResponseType)(RespType);
            }
            //Response Description
            if (newticket.ResponseDesc != RespDesc)
            {
                newticket.ResponseDesc = RespDesc;
            }
            //Hours Spent
            if (newticket.HoursSpent != HoursSpent)
            {
                newticket.HoursSpent = HoursSpent;
            }
            // Real Priority.
            if (newticket.RealPriority != (Priority)(RelPriority))
            {
                newticket.RealPriority = (Priority)(RelPriority);

                // Change the due date upon changing the real priority

                switch (newticket.RealPriority)
                {
                    case Priority.A_2days:
                        newticket.DueDate = newticket.CreatedDate.AddDays(2);
                        break;
                    case Priority.B_5days:
                        newticket.DueDate = newticket.CreatedDate.AddDays(5);
                        break;
                    case Priority.C_9days:
                        newticket.DueDate = newticket.CreatedDate.AddDays(9);
                        break;

                    default:
                        throw new Exception();
                }
            }
            if (Status == Status.Closed)
            {
                newticket.ClosedDate = DateTime.Now;
            }

            newticket.LastUpdated = DateTime.Now;


            // Update the dataase.
            try
            {
                _context.Update(newticket);
                _context.SaveChanges();

                // Generate Email while closing Ticket.
                if (Status == Status.Closed)
                {
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = newticket.Id }, protocol: Request.Scheme);
                    if (createdUser != null)
                    {
                        _emailSender.SendEmailAsync(
                               createdUser.Email,
                               "The Ticket is closed",
                               $"The ticket closed by { loggedInUser}. " +
                               $"See the ticket here: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> Details.");
                    }
                }
                return "The Ticket status successfully Upadated !!";
            }
            catch (DbUpdateConcurrencyException)
            {
                return "Ticket Not found";

            }
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

            ticket.Documents = await _context.Documents.Where(d => d.TicketId == id).ToListAsync();

            var model = new TicketDetailsViewModel
            {
                Ticket = ticket,
                Documents = ticket.Documents,
            };

            return View(model);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            List<Document> ticketdocuments = _context.Documents.Where(d => d.TicketId == id).ToList();

            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();            
            
            if (ticketdocuments.Count != 0) 
            {
                foreach (var documents in ticketdocuments)
                {
                    if (!System.IO.File.Exists(documents.Path))
                    {
                        NotFound();
                    }
                    else
                    {
                        try
                        {
                            System.IO.File.Delete(documents.Path);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }                
            }            

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

        public async Task<IActionResult> AddComment([Bind("Id,CommentTime,CommentBy,CommentText,TicketId")] Comment comment)
        {
            comment.CommentTime = DateTime.Now;
            loggedInUser = await userManager.GetUserAsync(User);
            comment.CommentBy = loggedInUser.Id;

            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = comment.TicketId });
        }

        private void Fileupload(List<IFormFile> inputFiles, long ticketid,string userid,string ticketRfno)
        {
            string FileName = null;
            string filePath = null;
            foreach (var inputFile in inputFiles)
            {
                if (inputFile != null)
                {
                    string projectDir = System.IO.Directory.GetCurrentDirectory();
                    var uploadsFolder = Path.Combine(projectDir, "wwwroot/Docs");
                    FileName = Path.GetFileName(inputFile.FileName);
                    filePath = Path.Combine(uploadsFolder, ticketRfno + "_" + FileName);

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    using (FileStream f = new FileStream(filePath, FileMode.Create))
                    {
                        inputFile.CopyTo(f);
                        f.Close();
                    }
                }

                var filename = ticketRfno + "_" + FileName;
                var document = new Document
                {
                    Name = filename,
                    UploadTime = DateTime.Now,
                    Path = filePath,
                    ApplicationUserId = userid,
                    TicketId = ticketid
                };

                if (ModelState.IsValid)
                {
                    _context.Add(document);
                    _context.SaveChanges();
                }
            }
        }
               
        [HttpPost]
        public JsonResult GetProjects(string customerId)
        {
            var selectedUserProjects = string.IsNullOrEmpty(customerId)?
                _context.Projects.Select(row => row)
                : _context.Projects.Where(g => g.CompanyId == _context.ApplicationUsers.FirstOrDefault(a => a.Id == customerId).CompanyId);


            List<SelectListItem> selectListProjects = new List<SelectListItem>();

            foreach (var project in selectedUserProjects)
            {
                var selectItem = new SelectListItem
                {
                    Text = project.Name,
                    Value = project.Id.ToString()
                };
                selectListProjects.Add(selectItem);
            }
            return Json(selectListProjects);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

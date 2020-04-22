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
        private static IEnumerable<ApplicationUser> confirmedCustomers;
        private List<ApplicationUser> ticketProjectDevelopers;        
        private readonly IEmailSender _emailSender;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            this.userManager = userManager;
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
            if (User.IsInRole("Admin"))
            {
                model = await TicketViewModelAdmin(model);
            }
            else if(User.IsInRole("Developer"))
            {
                model = await TicketViewModelDeveloper(model, loggedInUser);
            }
            else
            {
                model = await TicketViewModelCustomer(model, loggedInUser);
            }

            if (User.IsInRole("Developer") || User.IsInRole("Admin"))
            {
                ViewData["Companies"] = new SelectList(_context.Companies, "Id", "CompanyName");
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            }

            else if (User.IsInRole("Customer"))
            {
                //loggedInUser = await userManager.GetUserAsync(User);
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");
            }

            // Sort by attributes in the list
            //model = SortList(sortOrder, model);
            return View(model);
        }

        // Index for Customer Company Tickets  
        public async Task<IActionResult> IndexCompanyTickets(string sortOrder, List<TicketIndexViewModel> model, string title, int? statusSearch, int? customerPriority, int? priorities)
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");

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
                            UserEmail = s.AssignedUser.Email,
                            HoursSpent = s.HoursSpent
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
                        UserEmail = s.AssignedUser.Email,
                       
                    })
                    .ToListAsync();

            return model;
        }

        // Admin tickets ViewModel and List all the result and not show Draft status and Closed status
        private async Task<List<TicketIndexViewModel>> TicketViewModelAdmin(List<TicketIndexViewModel> model)
        {

            model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
            .Where(s => s.Status != Status.Utkast && s.Status != Status.Avslutat)
            .Select(s => new TicketIndexViewModel
            {
                Id = s.Id,
                RefNo = s.RefNo,
                Title = s.Title,
                Status = s.Status,
                ProjectName = s.Project.Name,
                CompanyName = s.Project.Company.CompanyName,
                CustomerPriority = s.CustomerPriority,
                RealPriority = s.RealPriority,
                DueDate = s.DueDate,
                UserEmail = s.AssignedUser.Email,
                HoursSpent = s.HoursSpent,
                ResponseType = s.ResponseType
            })
                .ToListAsync();
            
                return model;
        }

        private async Task<List<TicketIndexViewModel>> TicketViewModelDeveloper(List<TicketIndexViewModel> model, ApplicationUser loggedInUser)
        {   
                model = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
               .Where(u => u.AssignedTo == loggedInUser.Id)
               .Select(s => new TicketIndexViewModel
               {
                   Id = s.Id,
                   RefNo = s.RefNo,
                   Title = s.Title,
                   Status = s.Status,
                   ProjectName = s.Project.Name,
                   CompanyName = s.Project.Company.CompanyName,
                   CustomerPriority = s.CustomerPriority,
                   RealPriority = s.RealPriority,
                   DueDate = s.DueDate,
                   UserEmail = s.AssignedUser.Email,
                   HoursSpent = s.HoursSpent,
                   ResponseType = s.ResponseType
               })
                   .ToListAsync();

            return model;
        }
        //Filter by Title, Status and Priority
        public async Task<IActionResult> Filter(string companySearch, string projectSearch, string title, int? statusSearch, int? customerPriority, int? adminPriority, int? priorities, List<TicketIndexViewModel> model, List<TicketIndexViewModel> model2, string sortOrder, string currentFilter)
        {
            var loggedInUser = await userManager.GetUserAsync(User);

            // List result 
            if (User.IsInRole("Admin"))
            {
                model = await TicketViewModelAdmin(model);
            }
            else if (User.IsInRole("Developer"))
            {
                model = await TicketViewModelDeveloper(model, loggedInUser);
            }
            else
            {
                model = await TicketViewModelCustomer(model, loggedInUser);
            }

            if (User.IsInRole("Developer") || User.IsInRole("Admin"))
            {
                ViewData["Companies"] = new SelectList(_context.Companies, "Id", "CompanyName");
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            }

            else if (User.IsInRole("Customer"))
            {
                //loggedInUser = await userManager.GetUserAsync(User);
                ViewData["ProjectId"] = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");
            }

            // List all the result and not show Draft Status 
            model2 = await _context.Tickets.Include(t => t.AssignedUser).Include(t => t.CreatedUser).Include(t => t.Project)
                       .Where(s => s.Status != Status.Utkast)
                       .Select(s => new TicketIndexViewModel
                       {
                           Id = s.Id,
                           RefNo = s.RefNo,
                           Title = s.Title,
                           Status = s.Status,
                           ProjectName = s.Project.Name,
                           CompanyName = s.Project.Company.CompanyName,
                           CustomerPriority = s.CustomerPriority,
                           RealPriority = s.RealPriority,
                           DueDate = s.DueDate,
                           UserEmail = s.AssignedUser.Email,
                           HoursSpent = s.HoursSpent,
                           ResponseType = s.ResponseType
                       })
                       .ToListAsync();


            // Search by Title
            model = string.IsNullOrWhiteSpace(title) ?
            model :
            model.Where(p => p.Title.ToLower().Contains(title.ToLower())).ToList();

            // Search by Status Drowpdown
            if (User.IsInRole("Admin"))
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

            var projectsNameListAdmin = new SelectList(_context.Projects, "Id", "Name");
            var projectsNameListCustomer = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");

            if (User.IsInRole("Developer") || User.IsInRole("Admin"))
            {
                // Search by project Admin          
                foreach (var item in projectsNameListAdmin)
                {
                    if (item.Value == projectSearch)
                    {
                        model = model.Where(m => m.ProjectName == item.Text).ToList();
                    }
                }

            }
            else 
            {
                // Search by project Customer Drowpdown
                foreach (var item in projectsNameListCustomer)
                {
                    if (item.Value == projectSearch)
                    {
                        model = model.Where(m => m.ProjectName == item.Text).ToList();
                    }
                }

            }

            // Search by company Drowpdown
            var companiesNameList = new SelectList(_context.Companies, "Id", "CompanyName");
            foreach (var item in companiesNameList)
            {
                if (item.Value == companySearch)
                {
                    model = model.Where(m => m.CompanyName == item.Text).ToList();
                }
            }

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
        public async Task<IActionResult> FilterForCompanyTickets(string projectSearch, string title, int? statusSearch, int? customerPriority, int? adminPriority, int? priorities, List<TicketIndexViewModel> model, string sortOrder)
        {
            var loggedInUser = await userManager.GetUserAsync(User);
 
            ViewData["ProjectId"] = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");
           

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

            // Search by Status Drowpdown
            model = statusSearch == null ?
            model :
            model.Where(m => m.Status == (Status)statusSearch).ToList();

            // Search by Customer Priority Drowpdown
            model = customerPriority == null ?
            model :
            model.Where(m => m.CustomerPriority == (Priority)customerPriority).ToList();

            // Search by Real Priority Drowpdown
            model = adminPriority == null ?
            model :
            model.Where(m => m.RealPriority == (Priority)adminPriority).ToList();

            // Search by project Customer Drowpdown
            var projectsNameListCustomer = new SelectList(_context.Projects.Where(g => g.CompanyId == loggedInUser.CompanyId), "Id", "Name");

            foreach (var item in projectsNameListCustomer)
            {
                if (item.Value == projectSearch)
                {
                    model = model.Where(m => m.ProjectName == item.Text).ToList();
                }
            }

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
                ViewData["Companies"] = new SelectList(_context.Companies, "Id", "CompanyName");

                var customers = await userManager.GetUsersInRoleAsync("Customer");
                confirmedCustomers = customers.Where(a => a.EmailConfirmed == true);
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
            string companyLastRefNo = companyHasTicket == true ?
                _context.Tickets.Where(t => t.RefNo.Contains(companyAbbr)).ToList().LastOrDefault().RefNo
                : companyLastRefNo = companyAbbr + "00000";

            /*Increasing that last RefNo by 1 and assigning it to the newly added ticket*/
            model.Ticket.RefNo = Regex.Replace(companyLastRefNo, "\\d+",
                m => (int.Parse(m.Value) + 1).ToString(new string('0', m.Value.Length)));

            model.Ticket.CreatedDate = DateTime.Now;
            model.Ticket.LastUpdated = DateTime.Now;

            model.Ticket.RealPriority = model.Ticket.CustomerPriority;            
            
            switch (submit)
            {
                case "Skicka in":
                    model.Ticket.Status = Status.Inskickat;
                    model.Ticket.DueDate = DateTimeExtensions.SetDueDate(model.Ticket.CustomerPriority);
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

                case "Spara som Utkast":
                    model.Ticket.Status = Status.Utkast;
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
               
                if (model.Ticket.Status.Equals(Status.Inskickat))
                {
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = model.Ticket.Id }, protocol: Request.Scheme);
                    var ticketRefNo = model.Ticket.RefNo;

                    if (User.IsInRole("Admin") || User.IsInRole("Developer"))
                        return await SubmittedTicketEmail(loggedInUserCompany, callbackUrl, ticketRefNo, model.NotifyCustomer);
                    else if (User.IsInRole("Customer"))
                        return await SubmittedTicketEmail(loggedInUserCompany, callbackUrl, ticketRefNo, true);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /*Ticket has been assigned to developer1, but emails sent to both developer1 and developer2*/
        private async Task<IActionResult> SubmittedTicketEmail(Company loggedInUserCompany, string callbackUrl, string ticketRefNo, bool notifyCustomer)
        {
            if (notifyCustomer == true)
            {
                await _emailSender.SendEmailAsync(
                      loggedInUser.Email,
                      $"Ärendet {ticketRefNo} har mottagits",
                      $"Du kan inte svara på detta e-postutskick."+
                      $"<br/>Vänligen använd ärendehanteringssystemet för all skriftlig kommunikation i ärendet." +
                      $"<br/><br/>Hej {loggedInUser.FirstName}," +
                      $"<br/><br/>Ditt nya ärende med ärendenummer <b>{ticketRefNo}</b> har mottagits. " +
                      $"<br/>Klicka här för att se ärendet:<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> {ticketRefNo} </a>" +              
                      $"<br/><br/>Med vänliga hälsningar,<br/>Bitoreq Support");
            }
            foreach (var developer in ticketProjectDevelopers)
            {
                await _emailSender.SendEmailAsync(
                  developer.Email,
                  $"Ärendet {ticketRefNo} har skickats",
                  $"Du kan inte svara på detta e-postutskick." +
                  $"<br/>Vänligen använd ärendehanteringssystemet för all skriftlig kommunikation i ärendet." +
                  $"<br/><br/>Hej {developer.FirstName}," +
                  $"<br/><br/>Ett nytt ärende med ärendenummer <b>{ticketRefNo}</b> är insänt av {loggedInUser.Email} hos kund <b>{loggedInUserCompany.CompanyName}</b>." +
                  $"<br/>Klicka här för att se ärendet:<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> {ticketRefNo} </a>" +
                  $"<br/><br/>Med vänliga hälsningar,<br/>Bitoreq Support");
            }

            return RedirectToAction(nameof(EmailSent));
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
            model.Ticket.LastUpdated = DateTime.Now;            

            switch (submit)
            {
                case "Skicka in":
                    model.Ticket.Status = Status.Inskickat;
                    model.Ticket.CreatedDate = DateTime.Now;
                    model.Ticket.DueDate = DateTimeExtensions.SetDueDate(model.Ticket.CustomerPriority);
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

                case "Spara som Utkast":
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

                if (model.Ticket.Status.Equals(Status.Inskickat))
                {
                    Company loggedInUserCompany = _context.Companies.Find(loggedInUser.CompanyId);
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = model.Ticket.Id }, protocol: Request.Scheme);
                    var ticketRefNo = model.Ticket.RefNo;

                    return await SubmittedTicketEmail(loggedInUserCompany, callbackUrl, ticketRefNo, true);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // Edit Ticket Through Detail Screen.
        [HttpPost]
        public string SaveResponse( long id, double HoursSpent, Status Status, string RespDesc, ResponseType RespType, Priority RelPriority, DateTime DueD)
        {
            var priorityChange = false;
            Priority previousPriority = RelPriority;
            var newTicket = _context.Tickets.Find(id);

            string loggedInUser = (string)TempData["loggedInUser"];
            var createdUser = _context.ApplicationUsers.FirstOrDefault(a => a.Id == newTicket.CreatedBy);


            TempData.Keep();

            if (newTicket == null)
            {
                return "Ärende hittades inte";
            }

            /*Update only the changed value to database.*/

            /*Status*/
            if (newTicket.Status != (Status)(Status))
            {
                newTicket.Status = (Status)(Status);
            }

            /*Response Type*/
            if (newTicket.ResponseType != (ResponseType)(RespType))
                newTicket.ResponseType = (ResponseType)(RespType);
                

            /*Response Description*/
            if (newTicket.ResponseDesc != RespDesc)
                newTicket.ResponseDesc = RespDesc;

            /*Hours Spent*/
            if (newTicket.HoursSpent != HoursSpent)
                newTicket.HoursSpent = HoursSpent;

            /*Due Date*/
            if (newTicket.DueDate != DueD)
                newTicket.DueDate = DueD;

            /*Real Priority*/
            if (newTicket.RealPriority != (Priority)(RelPriority))
            {
                previousPriority = (Priority) newTicket.RealPriority;
                newTicket.RealPriority = (Priority)(RelPriority);
                priorityChange = true;

                /*Change the due date upon changing the real priority*/
                //newTicket.DueDate = DateTimeExtensions.SetDueDate(newTicket.RealPriority);
            }
            if (Status == Status.Avslutat)
            {
                newTicket.ClosedDate = DateTime.Now;
            }

            newTicket.LastUpdated = DateTime.Now;
            var callbackUrl1 = Url.Action("Details", "Tickets", new { id = newTicket.Id }, protocol: Request.Scheme);

            /*Update database*/
            try
            {
                _context.Update(newTicket);
                _context.SaveChanges();

                /*Generate Email when priority is changed.*/

               if(priorityChange == true)
                {
                    var ticketRefNo = newTicket.RefNo;

                    if (createdUser != null)
                    {
                        _emailSender.SendEmailAsync(
                               createdUser.Email,
                               $"Prioritetsändring för ärendet {ticketRefNo}",
                               $"Du kan inte svara på detta e-postutskick." +
                               $"<br/>Vänligen använd ärendehanteringssystemet för all skriftlig kommunikation i ärendet." +
                               $"<br/><br/>Hej {createdUser.FirstName}," +
                               $"<br/><br/>Prioriteten för ärendet {ticketRefNo} har ändrats från {previousPriority}  till {RelPriority} av { loggedInUser}. " +
                               $"<br/>Klicka här för att se ärendet:<a href='{HtmlEncoder.Default.Encode(callbackUrl1)}'> {ticketRefNo} </a>" +                             
                               $"<br/><br/>Med vänliga hälsningar,<br/>Bitoreq Support");
                    }

                }


                /*Generate Email while closing Ticket*/
                if (Status == Status.Avslutat)
                {                    
                    var callbackUrl = Url.Action("Details", "Tickets", new { id = newTicket.Id }, protocol: Request.Scheme);
                    var ticketRefNo = newTicket.RefNo;

                    if (createdUser != null)
                    {
                        _emailSender.SendEmailAsync(
                               createdUser.Email,
                               $"Ärendet {ticketRefNo} har avslutats",
                               $"Du kan inte svara på detta e-postutskick." +
                               $"<br/>Vänligen använd ärendehanteringssystemet för all skriftlig kommunikation i ärendet." +
                               $"<br/><br/>Hej {createdUser.FirstName}," +
                               $"<br/><br/>Ärendet med nummer {ticketRefNo} har avslutats av { loggedInUser}." +
                               $"<br/>Klicka här för att se ärendet:<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'> {ticketRefNo} </a>" +
                               $"<br/><br/>Med vänliga hälsningar,<br/>Bitoreq Support");
                    }   
                }
                return "Ärendestatus har uppdaterats!";
            }
            catch (DbUpdateConcurrencyException)
            {
                return "Ärende Inte hittat";
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

        private void Fileupload(List<IFormFile> inputFiles, long ticketid, string userid, string ticketRfno)
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
        public JsonResult GetCustomers(string companyId)
        {
            var compCustomers = confirmedCustomers;
            var selectedCompanyCustomers= string.IsNullOrEmpty(companyId) ?
                compCustomers
                : compCustomers.Where(g => g.CompanyId.ToString() == companyId);


            List<SelectListItem> selectListCustomers = new List<SelectListItem>();

            foreach (var customer in selectedCompanyCustomers)
            {
                var selectItem = new SelectListItem
                {
                    Text = customer.Email,
                    Value = customer.Id
                };
                selectListCustomers.Add(selectItem);
            }
            return Json(selectListCustomers);
        }

        [HttpPost]
        public JsonResult GetProjects(string customerId)
        {
            var selectedUserProjects = string.IsNullOrEmpty(customerId) ?
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

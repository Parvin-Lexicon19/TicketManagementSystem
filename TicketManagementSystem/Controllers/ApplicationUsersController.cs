﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Core.Models;
using TicketManagementSystem.Data;

namespace TicketManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        public static readonly string BITOREQNAME = "Bitoreq";

        public ApplicationUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        [Route("ApplicationUsers/Index/{name}")]
        public async Task<IActionResult> Index(string name)
        {
            var users=userManager.Users;
            if(name=="Developer")
            {
                 users = GetUsersBasedonRoles("Developer");
               
            }
            else if(name=="Customer")
            {
                 users = GetUsersBasedonRoles("Customer");
                
            }
            ViewData["PageName"] = name;
            return View(await users.ToListAsync());


        }


        //public async Task<IActionResult> CustomerIndex()
        //{
        //    var customerusers = GetUsersBasedonRoles("Customer");
        //    return View(await customerusers.ToListAsync());
        //}


        [Route("Edit/{id}/{name}")]
        // GET: ApplicationUser/Edit/5
        public async Task<IActionResult> Edit(string? id,string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            

            if (name=="Developer")
            {
                var companies = _context.Companies.Where(c => c.CompanyName == BITOREQNAME).Select(i => new SelectListItem()
                {
                    Text = i.CompanyAbbr,
                    Value = i.Id.ToString()
                });
                ViewData["CompanyId"] = companies;

            }
            else if(name == "Customer")
            {
                var companies = _context.Companies.Where(c => c.CompanyName != BITOREQNAME).Select(i => new SelectListItem()
                {
                    Text = i.CompanyAbbr,
                    Value = i.Id.ToString()
                });
                ViewData["CompanyId"] = companies;
            }



            
            return View(user);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit/{id}/{name}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string name, [Bind("Id,UserName,FirstName,LastName,Email,CompanyId,JobTitle,Country,PhoneNumber")] ApplicationUser applicationuser)
        {
            if (id != applicationuser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                try
                {

                    user.FirstName = applicationuser.FirstName;
                    user.LastName = applicationuser.LastName;
                    user.Email = applicationuser.Email;
                    user.UserName = applicationuser.UserName;
                    user.JobTitle = applicationuser.JobTitle;
                    user.Country = applicationuser.Country;
                    user.PhoneNumber = applicationuser.PhoneNumber;
                    user.CompanyId = applicationuser.CompanyId;

                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!ApplicationUserExists(applicationuser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        //Console.WriteLine("{0} Exception caught.", e.Message);
                        throw;
                    }
                }
                if (name == "Developer")
                {

                    return RedirectToAction("Index","ApplicationUsers", new { name = "Developer" });
                }
                else if (name == "Customer")
                {
                    return RedirectToAction("Index", "ApplicationUsers", new { name = "Customer" });
                }

                return RedirectToAction(nameof(Index));
                
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "CompanyAbbr", applicationuser.CompanyId);

            return View(applicationuser);
        }

        [Route("Details/{id}/{name}")]
        public async Task<IActionResult> Details(string? id,string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.Users
                .Include(p => p.Company)
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
            {
                return NotFound();
            }
            ViewData["PageName"] = name;
            return View(user);
        }

        [Route("Delete/{id}/{name}")]
        public async Task<IActionResult> Delete(string? id,string name)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["PageName"] = name;
            return View(user);
        }

        [Route("Delete/{id}/{name}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id,string name)
        {
            var user = await userManager.FindByIdAsync(id);
            _context.Remove(user);
            await _context.SaveChangesAsync();

            if (name == "Developer")
            {

                return RedirectToAction("Index", "ApplicationUsers", new { name = "Developer" });
            }
            else if (name == "Customer")
            {
                return RedirectToAction("Index", "ApplicationUsers", new { name = "Customer" });
            }

            return RedirectToAction(nameof(Index));
        }



        private IQueryable<ApplicationUser> GetUsersBasedonRoles(string rolename)
        {
            var roleId = _context.Roles.Where(r => r.Name == rolename).Select(r => r.Id).ToList();
            var useridList = _context.UserRoles.Where(x => roleId.Contains(x.RoleId)).Select(c => c.UserId).ToList();
            var users = userManager.Users.Include(c=>c.Company).Where(t => useridList.Contains(t.Id));
            return users;
        }

        private bool ApplicationUserExists(string id)
        {
            return userManager.Users.Any(e => e.Id == id);
        }

    }
}
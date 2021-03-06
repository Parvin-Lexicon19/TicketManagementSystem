﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using TicketManagementSystem.Core.Models;
using TicketManagementSystem.Data;

namespace TicketManagementSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string RoleName { get; set; }

        public SelectList CompanyOption { get; set; }
        public SelectList RoleOption { get; set; }


        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]

            [Display(Name = "Role")]
            public string Role { get; set; }
            [Required]

            [Display(Name = "FirstName")]
            [StringLength(50)]
            public string FirstName { get; set; }            

            [Required]
            [Display(Name = "LastName")]
            [StringLength(50)]

            public string LastName { get; set; }

            [Display(Name = "Country")]

            public string Country { get; set; }

            [Display(Name = "Phone Number")]
            [StringLength(30)]

            public string PhoneNumber { get; set; }

            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            [Required]
            [Display(Name = "Company Name")]
            public string CompanyName { get; set; }

      




        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            // pass the Role List using ViewData
            var getAllRoles = _context.Roles.OrderBy(x => x.Name);


            ViewData["roles"] = getAllRoles.ToList();

            // pass the Company List.Sort by company Name.

            var getAllCompanyName = _context.Companies.OrderBy(x => x.CompanyName);

            
            CompanyOption = new SelectList(getAllCompanyName.Select(x => x.CompanyName));


        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var role = _roleManager.FindByIdAsync(Input.Role).Result;
           // var role = _roleManager.FindByIdAsync(Input.Role).Result;

            if (ModelState.IsValid)
            {
                // Get Company Name 
                var userCompanyName = _context.Companies.FirstOrDefault(c => c.CompanyName == Input.CompanyName);
              
                // Assign the company id when you create user.

                var user = new ApplicationUser {
                    UserName = Input.Email, Email = Input.Email,CompanyId = userCompanyName.Id,PhoneNumber = Input.PhoneNumber,
                    Country = Input.Country,FirstName = Input.FirstName,LastName = Input.LastName,JobTitle = Input.JobTitle
                };
                var result = await _userManager.CreateAsync(user, Input.Password);

                // Add First Name and last name to claims to access in _login partial view.

                await _userManager.AddClaimAsync(user, new Claim("FirstName", user.FirstName));
                await _userManager.AddClaimAsync(user, new Claim("LastName", user.LastName));

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    // Assign Role to the user.
                    await _userManager.AddToRoleAsync(user, role.Name);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        //   await _signInManager.SignInAsync(user, isPersistent: false);
                        TempData["alertMessage"] = "User created Sucessfully";
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)

                {

                    ModelState.AddModelError(string.Empty, error.Description);

                }
            }

            // If we got this far, something failed, redisplay form

             await OnGetAsync();
            return Page();
        }
    }
}

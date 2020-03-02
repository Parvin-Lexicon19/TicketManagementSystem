using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Data
{
    public static class SeedData
    {
        public static readonly string BITOREQNAME="Bitoreq";

        internal static async Task InitializeAsync(IServiceProvider services, string adminPW)
        {
            var options = services.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            using (var context = new ApplicationDbContext(options))
            {

                //seed BITOREQ company data to database
                var foundcompany =await context.Companies.CountAsync(c=>c.CompanyName == BITOREQNAME);
                if (foundcompany==0)
                {
                    var Bitoreqcompany = new Company()
                    {
                        CompanyName = BITOREQNAME,
                        CompanyAbbr = "BIREQ"
                    };
                    context.Add(Bitoreqcompany);
                    context.SaveChanges();
                }
               



                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Adding the roles to the db
                var roleNames = new[] { "Admin", "Customer", "Developer" };

                foreach (var name in roleNames)
                {
                    // If the role exists just continue
                    if (await roleManager.RoleExistsAsync(name)) continue;

                    // Otherwise create the role
                    var role = new IdentityRole
                    {
                        Name = name
                    };
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                // Creating admins
                var adminEmails = new[] { "admin@bitoreq.se" };

                foreach (var email in adminEmails)
                {
                    var foundUser = await userManager.FindByEmailAsync(email);
                    var companyid = await context.Companies.FirstOrDefaultAsync(c => c.CompanyName == BITOREQNAME);
                    if (foundUser != null) continue;
                    else
                    {
                        await NewUser(adminPW, userManager, email,companyid.Id);
                    }
                }


                // Assigning roles for the admin users
                foreach (var email in adminEmails)
                {
                    var adminUser = await userManager.FindByEmailAsync(email);
                    var adminUserRole = await userManager.GetRolesAsync(adminUser);
                    if (adminUserRole.Count > 0) continue;
                    else
                    {

                        var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");

                        if (!addToRoleResult.Succeeded)
                        {
                            throw new Exception(string.Join("\n", addToRoleResult.Errors));
                        }

                    }

                }



                ////creating Developers
                //var developersEmails = new[] { "developer1@bitoreq.se", "developer2@bitoreq.se", "developer3@bitoreq.se", "developer4@bitoreq.se" };

                //foreach (var email in developersEmails)
                //{
                //    var foundUser = await userManager.FindByEmailAsync(email);
                //    var companyid = await context.Companies.FirstOrDefaultAsync(c => c.CompanyName == BITOREQNAME);
                //    if (foundUser != null) continue;
                //    else
                //    {
                //        await NewUser(adminPW, userManager, email, companyid.Id);
                //    }
                //}


                
                //foreach (var email in developersEmails)
                //{
                //    var developerUser = await userManager.FindByEmailAsync(email);
                //    var developerUserRole = await userManager.GetRolesAsync(developerUser);
                //    if (developerUserRole.Count > 0) continue;
                //    else
                //    {

                //        var addToRoleResult = await userManager.AddToRoleAsync(developerUser, "Developer");

                //        if (!addToRoleResult.Succeeded)
                //        {
                //            throw new Exception(string.Join("\n", addToRoleResult.Errors));
                //        }

                //    }

                //}


            }
        }


        private static async Task NewUser(string adminPW, UserManager<ApplicationUser> userManager, string email,int companyid )
        {


            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                CompanyId= companyid

            };

                var result = await userManager.CreateAsync(user, adminPW);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors));
            }
        }
    }

    
}
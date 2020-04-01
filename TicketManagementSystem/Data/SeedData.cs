using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Data
{
    public static class SeedData
    {
        public static readonly string BITOREQNAME = "Bitoreq";

        internal static async Task InitializeAsync(IServiceProvider services, string adminPW)
        {
            var options = services.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            using (var context = new ApplicationDbContext(options))
            {

                //seed BITOREQ company data to database
                var foundcompany = await context.Companies.CountAsync(c => c.CompanyName == BITOREQNAME);
                if (foundcompany == 0)
                {
                    var Bitoreqcompany = new Company()
                    {
                        CompanyName = BITOREQNAME,
                        CompanyAbbr = "BIREQ"
                    };
                    context.Add(Bitoreqcompany);
                    context.SaveChanges();
                }

                //Seed 2 companies data to database
                var companies = await context.Companies.CountAsync(c => c.CompanyName != BITOREQNAME);
                var fake = new Faker();
                var random = new Random();


                if (companies == 0)
                {
                    var newcompanies = new List<Company>();
                    for (int i = 0; i < 2; i++)
                    {
                        var companyname = fake.Company.CompanyName();
                        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                        var companyabbr = rgx.Replace(companyname, "");
                        var company = new Company
                        {
                            
                            CompanyName = companyname,
                            CompanyAbbr = companyabbr.Substring(0,5).ToUpper(),
                            Email = fake.Internet.Email(companyname),
                            Address = fake.Address.StreetAddress(),
                            City = fake.Address.City(),
                            Country = "Sweden"

                        };
                        newcompanies.Add(company);
                    }

                        context.AddRange(newcompanies);
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
                        await NewUser(adminPW, userManager, email, companyid.Id);
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

                //creating Developers
                var developersEmails = new[] { "developer1@bitoreq.se", "developer2@bitoreq.se", "developer3@bitoreq.se", "developer4@bitoreq.se" };

                foreach (var email in developersEmails)
                {
                    var foundUser = await userManager.FindByEmailAsync(email);
                    var companyid = await context.Companies.FirstOrDefaultAsync(c => c.CompanyName == BITOREQNAME);
                    if (foundUser != null) continue;
                    else
                    {
                        await NewUser(adminPW, userManager, email, companyid.Id);
                    }
                }


                foreach (var email in developersEmails)
                {
                    var developerUser = await userManager.FindByEmailAsync(email);
                    var developerUserRole = await userManager.GetRolesAsync(developerUser);
                    if (developerUserRole.Count > 0) continue;
                    else
                    {

                        var addToRoleResult = await userManager.AddToRoleAsync(developerUser, "Developer");

                        if (!addToRoleResult.Succeeded)
                        {
                            throw new Exception(string.Join("\n", addToRoleResult.Errors));
                        }

                    }

                }


                //Creating Customers
                var CustomersEmails = new[] { "Customer1@bitoreq.se", "Customer2@bitoreq.se", "Customer3@bitoreq.se", "Customer4@bitoreq.se" };
                var getCompanyIds = context.Companies.Where(c => c.CompanyName != BITOREQNAME).Select(v => v.Id).ToList();

                
                foreach (var email in CustomersEmails)
                {
                    var foundUser = await userManager.FindByEmailAsync(email);


                    
                    if (foundUser != null) continue;
                    else
                    {
                        
                        var randomCompanyId = getCompanyIds.OrderBy(x => random.Next()).Take(1).FirstOrDefault();

                        await NewUser(adminPW, userManager, email, randomCompanyId);
                    }
                }


                //Assigning Customers Roles
                foreach (var email in CustomersEmails)
                {
                    var CustomerUser = await userManager.FindByEmailAsync(email);
                    var CustomerUserRole = await userManager.GetRolesAsync(CustomerUser);
                    if (CustomerUserRole.Count > 0) continue;
                    else
                    {

                        var addToRoleResult = await userManager.AddToRoleAsync(CustomerUser, "Customer");

                        if (!addToRoleResult.Succeeded)
                        {
                            throw new Exception(string.Join("\n", addToRoleResult.Errors));
                        }
                    }

                }

                ////updating existing users with activeuserflag
                var userscount = userManager.Users.Count();

                var users = userManager.Users.Where(u => u.ActiveUser != true).ToList();
                if (userscount == users.Count)
                {

                    foreach (var user in users)
                    {
                        user.ActiveUser = true;
                        await userManager.UpdateAsync(user);
                        await context.SaveChangesAsync();
                    }
                }

                //Seed 4 Projects
                var Projects = await context.Projects.CountAsync();
                string roleid = context.Roles.Where(r => r.Name == "Developer").FirstOrDefault().Id;
                //var getCompanyIds = context.Companies.Where(c => c.CompanyName != BITOREQNAME).Select(v => v.Id).ToList();

                if (Projects == 0)
                {
                    var newprojects = new List<Project>();
                    foreach (var company in getCompanyIds)
                    {
                        for (int j = 1; j < 3; j++)
                        {
                            var getcopantabbr = context.Companies.Where(c => c.Id == company).Select(c => c.CompanyAbbr).FirstOrDefault();
                            List<string> deverlopers =await context.UserRoles.Where(r => r.RoleId == roleid).Select(r => r.UserId).ToListAsync();

                            int index = random.Next(deverlopers.Count);
                            string developeerid = deverlopers[index];

                            var project = new Project
                            {
                                Name = getcopantabbr + "Solution" + j,
                                CompanyId = company,
                                ReleaseDate = fake.Date.Past(),
                                Developer1 = developeerid
                            };
                            newprojects.Add(project);
                        }
                    }

                    context.AddRange(newprojects);
                    context.SaveChanges();
                }
            }
        }


        private static async Task NewUser(string adminPW, UserManager<ApplicationUser> userManager, string email, int companyid)
        {

            var fake = new Faker();
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                CompanyId = companyid,
                EmailConfirmed = true,
                FirstName = fake.Person.FirstName,
                LastName= fake.Person.LastName
            };


            var result = await userManager.CreateAsync(user, adminPW);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("\n", result.Errors));
            }
        }
    }
}
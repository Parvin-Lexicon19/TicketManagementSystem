using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Körs alltid först annars skrivs vår egen konfiguration över av default inställningarna i base 
            base.OnModelCreating(builder);

            builder.Entity<Ticket>()
                    .HasOne(m => m.CreatedUser)
                    .WithMany(t => t.TicketCreatedBy)
                    .HasForeignKey(m => m.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict); 

            builder.Entity<Ticket>()
                    .HasOne(m => m.AssignedUser)
                    .WithMany(t => t.TicketAssignedTo)
                    .HasForeignKey(m => m.AssignedTo)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Project>()
                    .HasOne(m => m.Developer1User)
                    .WithMany(t => t.ProjectDeveloper1)
                    .HasForeignKey(m => m.Developer1)
                    .IsRequired(true)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Project>()
                    .HasOne(m => m.Developer2User)
                    .WithMany(t => t.ProjectDeveloper2)
                    .HasForeignKey(m => m.Developer2)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

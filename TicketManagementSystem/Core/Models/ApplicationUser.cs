using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class ApplicationUser:IdentityUser
    {
        
        public int CompanyId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public string FullName { get => FirstName + " " + LastName; }
        public String JobTitle { get; set; }
        public String Country { get; set; }

        public ICollection<Ticket> TicketCreatedBy { get; set; }
        public ICollection<Ticket> TicketAssignedTo { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Project> ProjectDeveloper1 { get; set; }
        public ICollection<Project> ProjectDeveloper2 { get; set; }
        public Company Company { get; set; }
    }
}

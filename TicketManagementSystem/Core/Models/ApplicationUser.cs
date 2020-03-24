using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name = "User Name")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        [Display(Name = "Company Abbr")]
        public int CompanyId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String FullName { get => FirstName + " " + LastName; }

        [Display(Name = "Job Title")]
        public String JobTitle { get; set; }
        public String Country { get; set; }

        [Required(ErrorMessage ="Email Required")]
        [EmailAddress(ErrorMessage ="Enter Valid Email")]
        [Remote(action: "EmailExist", controller: "ApplicationUsers")]
        public override string Email
        {
            get { return base.Email; }
            set { base.Email = value; }
        }


        [Display(Name = "Phone Number")]
        public override string PhoneNumber
        {
            get { return base.PhoneNumber; }
            set { base.PhoneNumber = value; }
        }

        public ICollection<Ticket> TicketCreatedBy { get; set; }
        public ICollection<Ticket> TicketAssignedTo { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Project> ProjectDeveloper1 { get; set; }
        public ICollection<Project> ProjectDeveloper2 { get; set; }
        public Company Company { get; set; }
    }
}

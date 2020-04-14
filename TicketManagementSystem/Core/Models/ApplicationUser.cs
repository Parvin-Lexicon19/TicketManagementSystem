using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Display(Name = "Användarnamn")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }

        [Display(Name = "Kund Förkortning")]
        public int CompanyId { get; set; }
        [Display(Name = "Förnamn")]
        public String FirstName { get; set; }
        [Display(Name = "Efternamn")]
        public String LastName { get; set; }
        public String FullName { get => FirstName + " " + LastName; }

        [Display(Name = "Jobbtitel")]
        public String JobTitle { get; set; }
        [Display(Name = "Land")]
        public String Country { get; set; }

        [Required(ErrorMessage ="Email Required")]
        [EmailAddress(ErrorMessage ="Enter Valid Email")]
        [Remote(action: "EmailExist", controller: "ApplicationUsers" , AdditionalFields = "Id")]
        [Display(Name = "E-post")]
        public override string Email
        {
            get { return base.Email; }
            set { base.Email = value; }
        }


        [Display(Name = "Telefonnummer")]
        public override string PhoneNumber
        {
            get { return base.PhoneNumber; }
            set { base.PhoneNumber = value; }
        }

        [Required]
        [DefaultValue(true)]
        public bool ActiveUser { get; set; }

        public ICollection<Ticket> TicketCreatedBy { get; set; }
        public ICollection<Ticket> TicketAssignedTo { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Project> ProjectDeveloper1 { get; set; }
        public ICollection<Project> ProjectDeveloper2 { get; set; }
        [Display(Name = "Kunder")]
        public Company Company { get; set; }
    }
}

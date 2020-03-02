using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        [StringLength(5)]

        [Display(Name = "Company Abbr")]
        public string CompanyAbbr { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Range(10000, 99999)]

        [Display(Name = "Postal Code")]
        public int? PostalCode { get; set; }
        public string Country { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public ICollection<ApplicationUser> ApplicationUser { get; set; }        
        public ICollection<Project> Projects { get; set; }
    }
}

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
        [MinLength(5)]
        [MaxLength(5)]
        [Display(Name = "Kunder Förkortning")]
        public string CompanyAbbr { get; set; }
        [Required]
        [Display(Name = "Kundersnamn")]
        public string CompanyName { get; set; }
        [Display(Name = "Kontaktperson")]
        public string ContactPerson { get; set; }
        [Display(Name = "E-post")]
        public string Email { get; set; }
        public string Address { get; set; }
        [Display(Name = "Stad")]
        public string City { get; set; }
        [Range(10000, 99999)]
        [Display(Name = "Postnummer")]
        public int? PostalCode { get; set; }
        [Display(Name = "Land")]
        public string Country { get; set; }
        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }
        public ICollection<ApplicationUser> ApplicationUser { get; set; }        
        public ICollection<Project> Projects { get; set; }
    }
}

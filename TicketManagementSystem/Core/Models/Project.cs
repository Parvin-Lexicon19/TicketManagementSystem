using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Display(Name = "Company Name")]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(70)]
        public string Name { get; set; }
        public string Developer1 { get; set; }
        public string Developer2 { get; set; }
        [DataType(DataType.Date)] 
        [Required]
        public DateTime ReleaseDate { get; set; }

        [DataType(DataType.Date)]
        [Remote(action: "CheckLastUpdate", controller: "Projects", AdditionalFields = "ReleaseDate")]
        public DateTime? LastUpdate { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        [StringLength(100)]
        public string SystemsUsed { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public Company Company { get; set; }
        public virtual ApplicationUser Developer1User { get; set; }
        public virtual ApplicationUser Developer2User { get; set; }
    }
}

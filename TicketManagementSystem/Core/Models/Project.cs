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
        [Display(Name = "Kunder")]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(70)]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Display(Name = "Utvecklare1")]
        public string Developer1 { get; set; }
        [Display(Name = "Utvecklare2")]
        public string Developer2 { get; set; }

        [Display(Name = "Första Driftsättningsdatum")]
        [DataType(DataType.Date)] 
        [Required]
        public DateTime ReleaseDate { get; set; }
        [Display(Name = "Senaste Driftsättningsdatum")]
        [DataType(DataType.Date)]
        [Remote(action: "CheckLastUpdate", controller: "Projects", AdditionalFields = "ReleaseDate")]
        public DateTime? LastUpdate { get; set; }
        [StringLength(500)]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        [Display(Name = "System Som Används")]
        [StringLength(100)]
        public string SystemsUsed { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        [Display(Name = "Kund")]
        public Company Company { get; set; }
        public virtual ApplicationUser Developer1User { get; set; }
        public virtual ApplicationUser Developer2User { get; set; }
    }
}

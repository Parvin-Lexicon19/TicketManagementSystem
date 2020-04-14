using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Core.ViewModels
{
    public class TicketIndexViewModel
    {

       // public Ticket Ticket { get; set; }

        //public Company CustomerCompany { get; }

        public Int64 Id { get; set; }
        [Required]
        [DisplayName("Ärendenummer")]
        public string RefNo { get; set; }
        [Required]
        [DisplayName("Titel")]
        public string Title { get; set; }
        [DisplayName("Status")]
        public Status? Status { get; set; }
        public int ProjectId { get; set; }
        [DisplayName("Lösning")]
        public string ProjectName { get; set; }

        [Display(Name = "Kund")]
        public string CompanyName { get; set; }

        [DisplayName("Prioritet")]
        public Priority? CustomerPriority { get; set; }
        [DisplayName("Bitoreq Prioritet")]
        public Priority? RealPriority { get; set; }
        public string AssignedTo { get; set; }

        [DisplayName("Tim")]
        public double HoursSpent { get; set; }

        [DisplayName("Svarstyp")]
        public ResponseType ResponseType { get; set; }

        [Required]
        [DisplayName("Ansvarig")]
        public string UserEmail { get; set; }

        [DisplayName("Lösning")]
        public Project Project { get; set; }

        [DisplayName("Förfallodatum")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

      
    }
}

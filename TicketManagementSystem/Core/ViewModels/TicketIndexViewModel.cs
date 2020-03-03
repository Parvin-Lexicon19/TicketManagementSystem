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

        public Int64 Id { get; set; }
        [Required]
        [DisplayName("Ticket No.")]
        public string RefNo { get; set; }
        [Required]
        public string Title { get; set; }
        [DisplayName("Status")]
        public Status? Status { get; set; }
       // [DisplayName("Status")]
       // public Status? AdminStatus { get; set; }

       // public Status? Status { get; set; }
        public int ProjectId { get; set; }
        [DisplayName("Project")]
        public string ProjectName { get; set; }
        [DisplayName("Customer Priority")]
        public Priority? CustomerPriority { get; set; }
        [DisplayName("Real Priority")]
        public Priority? RealPriority { get; set; }
        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }

    }

}

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
        public int ProjectId { get; set; }
        [DisplayName("Project")]
        public string ProjectName { get; set; }
        [DisplayName("Priority")]
        public Priority? CustomerPriority { get; set; }
        [DisplayName("Real Priority")]
        public Priority? RealPriority { get; set; }
        public string AssignedTo { get; set; }
        [Required]
        [DisplayName("Assigned To")]
        public string UserEmail { get; set; }

        [DisplayName("Due Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
    }
}

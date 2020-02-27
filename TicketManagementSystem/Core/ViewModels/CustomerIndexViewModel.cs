using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Core.ViewModels
{
    public class CustomerIndexViewModel
    {

        public Int64 Id { get; set; }
        [Required]
        [DisplayName("Ticket No.")]
        public string RefNo { get; set; }
        [Required]
        public string Title { get; set; }
        public Status Status { get; set; }
        public int ProjectId { get; set; }
        [DisplayName("Project")]
        public string ProjectName { get; set; }
        public Priority CustomerPriority { get; set; }
        public DateTime DueDate { get; set; }

    }
}

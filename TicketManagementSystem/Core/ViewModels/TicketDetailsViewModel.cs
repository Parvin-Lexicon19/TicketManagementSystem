using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Core.ViewModels
{
    public class TicketDetailsViewModel
    {
        public Ticket Ticket { get; set; }
        public Comment Comment { get; set; }

        [Display(Name = "Attached Documents")]
        public ICollection<Document> Documents { get; set; }
    }
}

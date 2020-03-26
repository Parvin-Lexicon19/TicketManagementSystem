using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Core.ViewModels
{
    public class ApplicationUserwithRole
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string  Role { get; set; }
    }
}

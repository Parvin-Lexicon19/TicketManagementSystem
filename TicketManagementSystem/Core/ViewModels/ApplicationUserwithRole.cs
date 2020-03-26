using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;

namespace TicketManagementSystem.Core.ViewModels
{
    public class ApplicationUserwithRole
    {

        public ApplicationUser ApplicationUser { get; set; }
        [Display(Name = "Roll")]
        public string  Role { get; set; }
    }
}

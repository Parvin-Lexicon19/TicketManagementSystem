using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketManagementSystem.Core.ViewModels
{
    public class AddTicketViewModel
    {

        public Ticket  Ticket { get; set; }
        public Company CustomerCompany { get; }
        public bool NotifyCustomer { get; set; }
        public List<IFormFile> File { get; set; }

    }
}

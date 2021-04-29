using Microsoft.AspNetCore.Http;
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
        public List<IFormFile> File { get; set; }
        public CommentViewModel CommentVM { get; set; }

        [Display(Name = "Bifogade filer")]
        public ICollection<Document> Documents { get; set; }
    }
}

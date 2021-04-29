using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketManagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TicketManagementSystem.Core.ViewModels
{
    public class CommentViewModel
    {
        public Comment Comment { get; set; }
        public List<IFormFile> File { get; set; }
    }
}

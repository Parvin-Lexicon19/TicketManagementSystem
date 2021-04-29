﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //Returns filename without its Guid prefix
        public string GetName { get { return Name.Split('_', 2)[1]; }}
        public string Path { get; set; }
        public Int64 TicketId { get; set; }
        public Int64? CommentId { get; set; }
        public DateTime UploadTime { get; set; }
        public string Description { get; set; }
        public  string ApplicationUserId { get; set; }
        public Ticket Ticket { get; set; }
        public Comment Comment { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

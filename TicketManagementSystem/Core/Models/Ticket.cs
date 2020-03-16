﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class Ticket
    {
        public Int64 Id { get; set; }
        [DisplayName("Ticket No.")]
        public string RefNo { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Description")]
        public string Problem { get; set; }
        [DisplayName("Created By")]
        public string CreatedBy { get; set; }
        [DisplayName("Created Date")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Assigned To")]
        public string AssignedTo { get; set; }
        [DisplayName("Hours Spent")]
        public double HoursSpent { get; set; }
        [Required]
        public Status Status { get; set; }
        [DisplayName("Project")]
        public int ProjectId { get; set; }
        [Required]
        [DisplayName("Customer Priority")]
        public Priority CustomerPriority { get; set; }
        [DisplayName("Real Priority")]
        public Priority RealPriority { get; set; }
        [Required]
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }
        [DisplayName("Closed Date")]
        public DateTime ClosedDate { get; set; }
        [DisplayName("Last Updated")]
        public DateTime LastUpdated { get; set; }
        public ResponseType ResponseType { get; set; }
        [DisplayName("Response Desc.")]
        public string ResponseDesc { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Project Project { get; set; }
        [DisplayName("Created User")]
        public virtual ApplicationUser CreatedUser { get; set; }
        [DisplayName("Assigned User")]
        public virtual ApplicationUser AssignedUser { get; set; }
    }

    public enum Status
    {
        Draft,
        Submitted,
        //Received,
        Started,
        Closed 

    }
    public enum Priority
    {
        A_2days,
        B_5days,
        C_9days
    }

    public enum ResponseType
    {
        A,
        B,
        C,
        D,
        E
    }
}

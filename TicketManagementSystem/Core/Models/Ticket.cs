using System;
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
        [Required]
        [DisplayName("Ticket No.")]
        public string RefNo { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        public string Problem { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AssignedTo { get; set; }
        public double HoursSpent { get; set; }
        [Required]
        public Status Status { get; set; }
        public int ProjectId { get; set; }
        [Required]
        public Priority CustomerPriority { get; set; }
        public Priority RealPriority { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public ResponseType ResponseType { get; set; }
        public string ResponseDesc { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public Project Project { get; set; }
        public  virtual ApplicationUser CreatedUser { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
    }

    public enum Status
    {
        Draft,     //for customer
        Submitted, //for customer
        Received,  //for developer
        Started,   //for developer
        Closed     //for both
    }
    public enum Priority
    {
        A,
        B,
        C
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

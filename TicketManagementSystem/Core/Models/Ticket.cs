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
        [DisplayName("Biljettnummer")]
        public string RefNo { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(500)]
        [DisplayName("Beskrivning")]
        public string Problem { get; set; }
        [DisplayName("Skapad av")]
        public string CreatedBy { get; set; }
        [DisplayName("Skapat Datum")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Tilldelats")]
        public string AssignedTo { get; set; }
        [DisplayName("Tillbringade Timmar")]
        public double HoursSpent { get; set; }
        [Required]
        public Status Status { get; set; }
        [DisplayName("Projekt")]
        public int ProjectId { get; set; }
        [Required]
        [DisplayName("Prioritet")]
        public Priority CustomerPriority { get; set; }
        [DisplayName("Verklig Prioritering")]
        public Priority RealPriority { get; set; }
        [Required]
        [DisplayName("Förfallodatum")]
        public DateTime DueDate { get; set; }
        [DisplayName("Stängt Datum")]
        public DateTime ClosedDate { get; set; }
        [DisplayName("Senast Uppdaterad")]
        public DateTime LastUpdated { get; set; }
        [DisplayName("Svarstyp")]
        public ResponseType ResponseType { get; set; }
        [DisplayName("Svarbeskrivning")]
        public string ResponseDesc { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Comment> Comments { get; set; }
        [DisplayName("Projekt")]
        public Project Project { get; set; }
        [DisplayName("Skapad Användare")]
        public virtual ApplicationUser CreatedUser { get; set; }
        [DisplayName("Tilldelad Användare")]
        public virtual ApplicationUser AssignedUser { get; set; }
    }

    public enum Status
    {
        Utkast,
        Lämnats,
        //Received,
        Satteigång,
        Stängd

    }
    public enum Priority
    {
        A_2days,
        B_5days,
        C_9days
    }

    public enum ResponseType
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5
    }
}

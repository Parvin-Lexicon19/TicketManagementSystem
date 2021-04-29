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
        [DisplayName("Ärendenummer")]
        public string RefNo { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Rubrik")]
        public string Title { get; set; }
        [Required]        
        [DisplayName("Beskrivning")]
        public string Problem { get; set; }
        [DisplayName("Skapad Av")]
        public string CreatedBy { get; set; }
        [DisplayName("Inskickat datum")]
        public DateTime CreatedDate { get; set; }
        [DisplayName("Tilldelats")]
        public string AssignedTo { get; set; }
        [DisplayName("Tim")]
        //[RegularExpression(@"^\d{1,15}$", ErrorMessage = "Please enter up to 15 digits for a contact number")]
        //[RegularExpression("([0-9]*[,|.])?[0-9]+")]
        public double HoursSpent { get; set; }
        [Required]
        public Status Status { get; set; }
        [DisplayName("Lösning")]
        public int ProjectId { get; set; }
        [Required]

        [DisplayName("Prioritet")]
        public Priority CustomerPriority { get; set; }
        [DisplayName("Bitoreq Prioritet")]
        public Priority RealPriority { get; set; }
        [Required]
        [DisplayName("Förfallodatum")]
        public DateTime DueDate { get; set; }
        [DisplayName("Avslutat datum")]
        public DateTime ClosedDate { get; set; }
        [DisplayName("Senast uppdaterat")]
        public DateTime LastUpdated { get; set; }
        [DisplayName("Svarstyp")]
        public ResponseType ResponseType { get; set; }
        [DisplayName("Svar")]
        public string ResponseDesc { get; set; }
        public ICollection<Document> Documents { get; set; }
        [DisplayName("Kommentarer")]
        public ICollection<Comment> Comments { get; set; }
        [DisplayName("Lösning")]
        public Project Project { get; set; }
        [DisplayName("Skapat av")]
        public virtual ApplicationUser CreatedUser { get; set; }
        //[DisplayName("Tilldelad Användare")]
        [DisplayName("Ansvarig")]
        public virtual ApplicationUser AssignedUser { get; set; }
    }

    public enum Status
    {
        Utkast,
        Inskickat,
        Påbörjat,
        Avslutat
    }

    //public enum Status
    //{
    //    Draft,
    //    Submitted,
    //    Started,
    //    Closed

    //}
    /*Assigned numbers are number of work days, you may change them if you want*/
    public enum Priority
    {
        //[Display(Name = "A: 2dgr")]
        A = 2,
        //[Display(Name = "B: 5dgr")]
        B = 5,
        //[Display(Name = "C: 9dgr")]
        C = 9
    }

    public enum ResponseType
    {
        [Display(Name = "Nytt krav")]
        Nyttkrav = 1,
        Felrättning = 2,
        [Display(Name = "Handhavande fel")]
        Handhavandefel = 3,
        [Display(Name = "Otillräcklig information")]
        Otillräckliginformation = 4,
        [Display(Name = "Fel i omgivande system")]
        Feliomgivandesystem = 5,
        [Display(Name = "Ändring i omgivande system")]
        Ändringiomgivandesystem =6,
        Annat = 7
    }
}

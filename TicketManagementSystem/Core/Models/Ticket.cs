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
        [DisplayName("Bitoreq Prioritet")]
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
        [DisplayName("kommentarer")]
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
        Satteigång,
        Stängd
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
        A = 2,
        B = 5,
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

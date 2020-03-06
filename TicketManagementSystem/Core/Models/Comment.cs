using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagementSystem.Core.Models
{
    public class Comment
    {
        public Int64 Id { get; set; }
        [Required]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy HH:mm}",
            ApplyFormatInEditMode = true)]
        public DateTime CommentTime { get; set; }
        [ForeignKey("ApplicationUser")]
        public string CommentBy { get; set; }
        [Required]
        [StringLength(500)]
        public string CommentText { get; set; }
        public Int64 TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

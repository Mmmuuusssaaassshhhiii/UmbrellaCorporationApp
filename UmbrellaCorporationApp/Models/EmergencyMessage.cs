using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmbrellaCorp.Models
{
    public class EmergencyMessage
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }

        public virtual Employee Sender { get; set; } = null!;
        
        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }

        public virtual Employee Receiver { get; set; } = null!;
        
        [Required]
        public string Text { get; set; } = string.Empty;
        
        public DateTime SentAt { get; set; } = DateTime.Now;
        
        public bool IsRead { get; set; }
        
        public bool IsEdited { get; set; }

        public bool IsDeleted { get; set; }
    }
}
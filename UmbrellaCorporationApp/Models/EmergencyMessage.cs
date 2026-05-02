using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmbrellaCorp.Models
{
    public class EmergencyMessage
    {
        [Key]
        public int Id { get; set; }
        public string MessageType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        [ForeignKey("Employee")]
        public int? SentById { get; set; }
        public virtual Employee? SentBy { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsAcknowledged { get; set; } = false;
    }
}
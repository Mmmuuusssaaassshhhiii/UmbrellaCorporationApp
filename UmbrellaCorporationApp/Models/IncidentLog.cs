using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class IncidentLog
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public IncidentSeverity Severity { get; set; }
        public string Description { get; set; } = string.Empty;

        [ForeignKey("Employee")]
        public int? ReportedById { get; set; }
        public virtual Employee? ReportedBy { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.Now;
        public bool IsResolved { get; set; } = false;
    }
}
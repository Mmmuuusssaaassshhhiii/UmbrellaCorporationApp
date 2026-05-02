using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmbrellaCorp.Models
{
    public class Statistic
    {
        [Key]
        public int Id { get; set; }
        public string MetricName { get; set; } = string.Empty;
        public decimal Value { get; set; }

        [ForeignKey("Virus")]
        public int? VirusId { get; set; }
        public virtual Virus? Virus { get; set; }

        public DateTime RecordedAt { get; set; } = DateTime.Now;
    }
}
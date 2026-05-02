using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class Development
    {
        [Key]
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        [ForeignKey("Virus")]
        public int? VirusId { get; set; }
        public virtual Virus? Virus { get; set; }

        [ForeignKey("Employee")]
        public int? LeadScientistId { get; set; }
        public virtual Employee? LeadScientist { get; set; }

        public DevelopmentStatus Status { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
    }
}
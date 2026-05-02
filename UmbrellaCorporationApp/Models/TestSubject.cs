using System.ComponentModel.DataAnnotations;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class TestSubject
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public int? VirusId { get; set; }
        public Virus? Virus { get; set; }

        public SubjectStatus Status { get; set; } = SubjectStatus.Alive;

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public DateTime AcquiredDate { get; set; }

        public string? Notes { get; set; }

        public List<LabReport> LabReports { get; set; } = new();
    }
}
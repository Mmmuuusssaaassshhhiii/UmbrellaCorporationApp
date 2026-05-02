using System.ComponentModel.DataAnnotations;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;

        public int ClearanceLevel { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Alive;

        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        [MaxLength(50)]
        public string? BadgeId { get; set; }

        public string? PhotoPath { get; set; }

        // Навигация
        public List<LabReport> LabReports { get; set; } = new();
        public List<Sample> Samples { get; set; } = new();
        public List<Development> Developments { get; set; } = new();
        public List<IncidentLog> IncidentLogs { get; set; } = new();
    }
}
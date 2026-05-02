using System.ComponentModel.DataAnnotations;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class LabReport
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public int AuthorId { get; set; }
        public Employee? Author { get; set; }

        public int? SampleId { get; set; }
        public Sample? Sample { get; set; }

        public int? TestSubjectId { get; set; }
        public TestSubject? TestSubject { get; set; }

        public DateTime CreatedAt { get; set; }

        public ConfidentialityLevel ConfidentialityLevel { get; set; }
    }
}
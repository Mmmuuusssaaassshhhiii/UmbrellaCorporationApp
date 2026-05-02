using System.ComponentModel.DataAnnotations;

namespace UmbrellaCorp.Models
{
    public class Sample
    {
        [Key]
        public int Id { get; set; }

        public int VirusId { get; set; }
        public Virus? Virus { get; set; }

        public int? ResponsibleScientistId { get; set; }
        public Employee? ResponsibleScientist { get; set; }

        [MaxLength(100)]
        public string StorageLocation { get; set; } = string.Empty;

        public bool IsDestroyed { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Notes { get; set; }
    }
}
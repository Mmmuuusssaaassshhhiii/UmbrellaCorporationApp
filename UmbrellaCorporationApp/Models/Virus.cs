using System.ComponentModel.DataAnnotations;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class Virus
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public VirusDangerLevel DangerLevel { get; set; } = VirusDangerLevel.Medium;

        public int IncubationHours { get; set; }

        public string? Symptoms { get; set; }

        public bool AntidoteExists { get; set; }

        public List<Sample> Samples { get; set; } = new();
        public List<TestSubject> TestSubjects { get; set; } = new();
    }
}
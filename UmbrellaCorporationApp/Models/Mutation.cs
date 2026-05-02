using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmbrellaCorp.Models
{
    public class Mutation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TestSubject")]
        public int? TestSubjectId { get; set; }
        public virtual TestSubject? TestSubject { get; set; }

        [ForeignKey("Virus")]
        public int? VirusId { get; set; }
        public virtual Virus? Virus { get; set; }

        public string ChangeDescription { get; set; } = string.Empty;
        public DateTime ObservedAt { get; set; } = DateTime.Now;
    }
}
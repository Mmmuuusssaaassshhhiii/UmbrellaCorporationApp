using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UmbrellaCorp.Models.Enums;

namespace UmbrellaCorp.Models
{
    public class ClassifiedFile
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public FileLevel Level { get; set; }
        public string Content { get; set; } = string.Empty;

        [ForeignKey("Employee")]
        public int AuthorId { get; set; }
        public virtual Employee? Author { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
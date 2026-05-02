using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmbrellaCorp.Models
{
    public class Archive
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(100)]
        public string SourceTable { get; set; }
        public int SourceRecordId { get; set; }
        [Column(TypeName = "longtext")]
        public string DataSnapshot { get; set; }
        public DateTime ArchivedAt { get; set; } = DateTime.Now;
        public string ArchiveReason { get; set; } = string.Empty;
    }
}
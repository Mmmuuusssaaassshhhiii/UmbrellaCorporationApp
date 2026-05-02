using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UmbrellaCorp.Models
{
    public class Loss
    {
        [Key]
        public int Id { get; set; }
        public string LossType { get; set; } = string.Empty;

        [ForeignKey("Sample")]
        public int? SampleId { get; set; }
        public virtual Sample? Sample { get; set; }

        [ForeignKey("TestSubject")]
        public int? TestSubjectId { get; set; }
        public virtual TestSubject? TestSubject { get; set; }

        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        public string Reason { get; set; } = string.Empty;
        public DateTime LossDate { get; set; } = DateTime.Now;
    }
}
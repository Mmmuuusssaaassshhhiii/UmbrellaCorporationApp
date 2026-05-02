using System.ComponentModel.DataAnnotations;

namespace UmbrellaCorp.Models
{
    public class EmergencyProtocol
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
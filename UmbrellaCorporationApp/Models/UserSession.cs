namespace UmbrellaCorporationApp.Models;

public class UserSession
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string IpAddress { get; set; } = "";
}
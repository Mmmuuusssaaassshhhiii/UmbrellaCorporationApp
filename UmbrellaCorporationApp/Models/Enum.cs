namespace UmbrellaCorp.Models.Enums
{
    public enum EmployeeStatus
    {
        Alive,
        Deceased,
        Infected,
        Missing
    }

    public enum SubjectStatus
    {
        Alive,
        Infected,
        Mutated,
        Deceased
    }

    public enum VirusDangerLevel
    {
        Low,
        Medium,
        High,
        Extreme
    }

    public enum ConfidentialityLevel
    {
        Public,
        Internal,
        Confidential,
        TopSecret
    }

    public enum IncidentSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum DevelopmentStatus
    {
        Active,
        Paused,
        Completed,
        Failed
    }

    public enum FileLevel
    {
        Confidential,
        Secret,
        TopSecret
    }
}
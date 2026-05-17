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
    
    public enum ClearanceLevel
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Level6 = 6,
        Level7 = 7,
        Level8 = 8,
        Level9 = 9,
        Level10 = 10
    }
}
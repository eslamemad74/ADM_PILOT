using AdmWorksheet.API.Models;

namespace AdmWorksheet.API.DTOs
{
    // ─── Pilot ───────────────────────────────────────────────────────────────
    public class PilotCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public double TotalFlightHours { get; set; }
    }

    public class PilotResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public double TotalFlightHours { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ─── Session ──────────────────────────────────────────────────────────────
    public class SessionCreateDto
    {
        public int PilotId { get; set; }
        public DateTime FlightDate { get; set; } = DateTime.UtcNow;
    }

    public class SessionResponseDto
    {
        public int Id { get; set; }
        public int PilotId { get; set; }
        public string PilotName { get; set; } = string.Empty;
        public DateTime FlightDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasImsafe { get; set; }
        public bool HasPave { get; set; }
        public bool HasDecide { get; set; }
        public bool HasFinalDecision { get; set; }
    }

    // ─── IMSAFE ───────────────────────────────────────────────────────────────
    public class ImsafeDto
    {
        public int SessionId { get; set; }

        public RiskLevel IllnessRating { get; set; }
        public string? IllnessNotes { get; set; }

        public RiskLevel MedicationRating { get; set; }
        public string? MedicationNotes { get; set; }

        public RiskLevel StressRating { get; set; }
        public string? StressNotes { get; set; }

        public RiskLevel AlcoholRating { get; set; }
        public string? AlcoholNotes { get; set; }

        public RiskLevel FatigueRating { get; set; }
        public string? FatigueNotes { get; set; }

        public RiskLevel EmotionRating { get; set; }
        public string? EmotionNotes { get; set; }
    }

    // ─── PAVE ─────────────────────────────────────────────────────────────────
    public class PaveDto
    {
        public int SessionId { get; set; }

        public string? PilotReadinessNotes { get; set; }
        public RiskLevel PilotRating { get; set; }

        public string? AircraftConditionNotes { get; set; }
        public RiskLevel AircraftRating { get; set; }

        public string? EnvironmentStatusNotes { get; set; }
        public RiskLevel EnvironmentRating { get; set; }

        public string? ExternalPressuresNotes { get; set; }
        public RiskLevel ExternalPressuresRating { get; set; }
    }

    // ─── DECIDE ───────────────────────────────────────────────────────────────
    public class DecideDto
    {
        public int SessionId { get; set; }

        public string? DetectNotes { get; set; }
        public RiskLevel DetectRating { get; set; }

        public RiskLevel EvaluateSeverity { get; set; }
        public RiskLevel EvaluateLikelihood { get; set; }
        // EvaluateRiskScore is computed server-side

        public List<string> ConsiderOptions { get; set; } = new();

        public string? IntegrateNotes { get; set; }
        public string? DecideAction { get; set; }

        public string? ExecuteNotes { get; set; }
        public string? ReassessNotes { get; set; }
    }

    // ─── Full Session Report ───────────────────────────────────────────────────
    public class SessionReportDto
    {
        public SessionResponseDto Session { get; set; } = new();
        public PilotResponseDto Pilot { get; set; } = new();
        public ImsafeDto? Imsafe { get; set; }
        public PaveDto? Pave { get; set; }
        public DecideReportDto? Decide { get; set; }
        public FinalDecisionDto? FinalDecision { get; set; }
    }

    public class DecideReportDto : DecideDto
    {
        public RiskLevel EvaluateRiskScore { get; set; }
    }

    public class FinalDecisionDto
    {
        public int SessionId { get; set; }
        public RiskLevel OverallRisk { get; set; }
        public GoNoGoDecision GoNoGo { get; set; }
        public string? RecommendationNotes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
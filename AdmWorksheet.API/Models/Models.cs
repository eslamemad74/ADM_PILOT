using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdmWorksheet.API.Models
{
    public enum RiskLevel { Low, Medium, High }
    public enum GoNoGoDecision { Go, NoGo, Review }

    // ─── Pilot ───────────────────────────────────────────────────────────────
    public class Pilot
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

        [Required, MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required]
        public double TotalFlightHours { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }

    // ─── Session ──────────────────────────────────────────────────────────────
    public class Session
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PilotId { get; set; }

        [ForeignKey(nameof(PilotId))]
        public Pilot? Pilot { get; set; }

        public DateTime FlightDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ImsafeChecklist? ImsafeChecklist { get; set; }
        public PaveChecklist? PaveChecklist { get; set; }
        public DecideModel? DecideModel { get; set; }
        public FinalDecision? FinalDecision { get; set; }
    }

    // ─── IMSAFE ───────────────────────────────────────────────────────────────
    public class ImsafeChecklist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session? Session { get; set; }

        // Illness
        public RiskLevel IllnessRating { get; set; }
        public string? IllnessNotes { get; set; }

        // Medication
        public RiskLevel MedicationRating { get; set; }
        public string? MedicationNotes { get; set; }

        // Stress
        public RiskLevel StressRating { get; set; }
        public string? StressNotes { get; set; }

        // Alcohol
        public RiskLevel AlcoholRating { get; set; }
        public string? AlcoholNotes { get; set; }

        // Fatigue
        public RiskLevel FatigueRating { get; set; }
        public string? FatigueNotes { get; set; }

        // Emotions
        public RiskLevel EmotionRating { get; set; }
        public string? EmotionNotes { get; set; }
    }

    // ─── PAVE ─────────────────────────────────────────────────────────────────
    public class PaveChecklist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session? Session { get; set; }

        // Pilot element
        public string? PilotReadinessNotes { get; set; }
        public RiskLevel PilotRating { get; set; }

        // Aircraft element
        public string? AircraftConditionNotes { get; set; }
        public RiskLevel AircraftRating { get; set; }

        // enVironment element
        public string? EnvironmentStatusNotes { get; set; }
        public RiskLevel EnvironmentRating { get; set; }

        // External Pressures element
        public string? ExternalPressuresNotes { get; set; }
        public RiskLevel ExternalPressuresRating { get; set; }
    }

    // ─── DECIDE ───────────────────────────────────────────────────────────────
    public class DecideModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session? Session { get; set; }

        // 1. Detect
        public string? DetectNotes { get; set; }
        public RiskLevel DetectRating { get; set; }

        // 2. Evaluate (two-dimensional: severity × likelihood)
        public RiskLevel EvaluateSeverity { get; set; }
        public RiskLevel EvaluateLikelihood { get; set; }
        public RiskLevel EvaluateRiskScore { get; set; }   // computed

        // 3. Consider (at least 3 options stored as JSON array string)
        public string? ConsiderOptions { get; set; }

        // 4. Integrate
        public string? IntegrateNotes { get; set; }

        // 5. Decide
        public string? DecideAction { get; set; }

        // 6. Execute & Reassess
        public string? ExecuteNotes { get; set; }
        public string? ReassessNotes { get; set; }
    }

    // ─── Final Decision ───────────────────────────────────────────────────────
    public class FinalDecision
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SessionId { get; set; }

        [ForeignKey(nameof(SessionId))]
        public Session? Session { get; set; }

        public RiskLevel OverallRisk { get; set; }
        public GoNoGoDecision GoNoGo { get; set; }
        public string? RecommendationNotes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
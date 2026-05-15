using AdmWorksheet.API.Models;
using AdmWorksheet.API.DTOs;
using System.Text.Json;

namespace AdmWorksheet.API.Services
{
    /// <summary>
    /// Implements the GRM risk matrix and Go/No-Go logic exactly as described in the PDF.
    /// Severity × Likelihood → Risk Score
    /// Any single "High" in IMSAFE or PAVE triggers a Go/No-Go review point.
    /// </summary>
    public class RiskService
    {
        // GRM Matrix: Severity × Likelihood → Risk Score
        // High×High=High, High×Med=High, High×Low=Medium
        // Med×High=High,  Med×Med=Medium, Med×Low=Low
        // Low×High=Medium, Low×Med=Low, Low×Low=Low
        public static RiskLevel ComputeRiskScore(RiskLevel severity, RiskLevel likelihood)
        {
            return (severity, likelihood) switch
            {
                (RiskLevel.High, RiskLevel.High) => RiskLevel.High,
                (RiskLevel.High, RiskLevel.Medium) => RiskLevel.High,
                (RiskLevel.High, RiskLevel.Low) => RiskLevel.Medium,
                (RiskLevel.Medium, RiskLevel.High) => RiskLevel.High,
                (RiskLevel.Medium, RiskLevel.Medium) => RiskLevel.Medium,
                (RiskLevel.Medium, RiskLevel.Low) => RiskLevel.Low,
                (RiskLevel.Low, RiskLevel.High) => RiskLevel.Medium,
                (RiskLevel.Low, RiskLevel.Medium) => RiskLevel.Low,
                (RiskLevel.Low, RiskLevel.Low) => RiskLevel.Low,
                _ => RiskLevel.Low
            };
        }

        // Compute overall risk from all checklist ratings
        public static (RiskLevel overall, GoNoGoDecision decision, string notes) ComputeFinalDecision(
            ImsafeChecklist imsafe,
            PaveChecklist pave,
            DecideModel decide)
        {
            var allRatings = new List<RiskLevel>
            {
                imsafe.IllnessRating,
                imsafe.MedicationRating,
                imsafe.StressRating,
                imsafe.AlcoholRating,
                imsafe.FatigueRating,
                imsafe.EmotionRating,
                pave.PilotRating,
                pave.AircraftRating,
                pave.EnvironmentRating,
                pave.ExternalPressuresRating,
                decide.EvaluateRiskScore
            };

            bool hasHigh = allRatings.Any(r => r == RiskLevel.High);
            bool hasMedium = allRatings.Any(r => r == RiskLevel.Medium);
            int highCount = allRatings.Count(r => r == RiskLevel.High);

            RiskLevel overall;
            GoNoGoDecision goNoGo;
            string notes;

            if (hasHigh)
            {
                overall = RiskLevel.High;
                goNoGo = highCount >= 2 ? GoNoGoDecision.NoGo : GoNoGoDecision.Review;
                notes = highCount >= 2
                    ? "Multiple HIGH-risk factors detected. Flight should be cancelled (No-Go). Address all high-risk items before rescheduling."
                    : "At least one HIGH-risk factor detected. Mandatory Go/No-Go review required. Do not proceed without mitigating high-risk items per PAVE and IMSAFE guidance.";
            }
            else if (hasMedium)
            {
                overall = RiskLevel.Medium;
                goNoGo = GoNoGoDecision.Review;
                notes = "Medium-risk factors present. Review stress multiplier effect: high stress + marginal conditions = unacceptable risk. Apply personal minimums before proceeding.";
            }
            else
            {
                overall = RiskLevel.Low;
                goNoGo = GoNoGoDecision.Go;
                notes = "All factors within acceptable limits. Cleared to proceed. Continue DECIDE loop monitoring throughout flight. Reassess at top-of-descent.";
            }

            return (overall, goNoGo, notes);
        }

        public static string SerializeOptions(List<string> options) =>
            JsonSerializer.Serialize(options);

        public static List<string> DeserializeOptions(string? json)
        {
            if (string.IsNullOrEmpty(json)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(json) ?? new(); }
            catch { return new List<string>(); }
        }
    }
}
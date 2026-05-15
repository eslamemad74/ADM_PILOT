using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdmWorksheet.API.Data;
using AdmWorksheet.API.Models;
using AdmWorksheet.API.DTOs;
using AdmWorksheet.API.Services;

namespace AdmWorksheet.API.Controllers
{
    // ══════════════════════════════════════════════════════════
    //  IMSAFE Controller
    // ══════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/[controller]")]
    public class ImsafeController : ControllerBase
    {
        private readonly AdmDbContext _db;
        public ImsafeController(AdmDbContext db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<ImsafeDto>> Submit([FromBody] ImsafeDto dto)
        {
            var session = await _db.Sessions
                .Include(s => s.ImsafeChecklist)
                .FirstOrDefaultAsync(s => s.Id == dto.SessionId);
            if (session is null) return NotFound("Session not found.");

            if (session.ImsafeChecklist is not null)
                _db.ImsafeChecklists.Remove(session.ImsafeChecklist);

            var entity = new ImsafeChecklist
            {
                SessionId = dto.SessionId,
                IllnessRating = dto.IllnessRating,
                IllnessNotes = dto.IllnessNotes,
                MedicationRating = dto.MedicationRating,
                MedicationNotes = dto.MedicationNotes,
                StressRating = dto.StressRating,
                StressNotes = dto.StressNotes,
                AlcoholRating = dto.AlcoholRating,
                AlcoholNotes = dto.AlcoholNotes,
                FatigueRating = dto.FatigueRating,
                FatigueNotes = dto.FatigueNotes,
                EmotionRating = dto.EmotionRating,
                EmotionNotes = dto.EmotionNotes
            };
            _db.ImsafeChecklists.Add(entity);
            await _db.SaveChangesAsync();
            return Ok(dto);
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<ImsafeDto>> Get(int sessionId)
        {
            var e = await _db.ImsafeChecklists.FirstOrDefaultAsync(i => i.SessionId == sessionId);
            if (e is null) return NotFound();
            return new ImsafeDto
            {
                SessionId = e.SessionId,
                IllnessRating = e.IllnessRating,
                IllnessNotes = e.IllnessNotes,
                MedicationRating = e.MedicationRating,
                MedicationNotes = e.MedicationNotes,
                StressRating = e.StressRating,
                StressNotes = e.StressNotes,
                AlcoholRating = e.AlcoholRating,
                AlcoholNotes = e.AlcoholNotes,
                FatigueRating = e.FatigueRating,
                FatigueNotes = e.FatigueNotes,
                EmotionRating = e.EmotionRating,
                EmotionNotes = e.EmotionNotes
            };
        }
    }

    // ══════════════════════════════════════════════════════════
    //  PAVE Controller
    // ══════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/[controller]")]
    public class PaveController : ControllerBase
    {
        private readonly AdmDbContext _db;
        public PaveController(AdmDbContext db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<PaveDto>> Submit([FromBody] PaveDto dto)
        {
            var session = await _db.Sessions
                .Include(s => s.PaveChecklist)
                .FirstOrDefaultAsync(s => s.Id == dto.SessionId);
            if (session is null) return NotFound("Session not found.");

            if (session.PaveChecklist is not null)
                _db.PaveChecklists.Remove(session.PaveChecklist);

            var entity = new PaveChecklist
            {
                SessionId = dto.SessionId,
                PilotReadinessNotes = dto.PilotReadinessNotes,
                PilotRating = dto.PilotRating,
                AircraftConditionNotes = dto.AircraftConditionNotes,
                AircraftRating = dto.AircraftRating,
                EnvironmentStatusNotes = dto.EnvironmentStatusNotes,
                EnvironmentRating = dto.EnvironmentRating,
                ExternalPressuresNotes = dto.ExternalPressuresNotes,
                ExternalPressuresRating = dto.ExternalPressuresRating
            };
            _db.PaveChecklists.Add(entity);
            await _db.SaveChangesAsync();
            return Ok(dto);
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<PaveDto>> Get(int sessionId)
        {
            var e = await _db.PaveChecklists.FirstOrDefaultAsync(p => p.SessionId == sessionId);
            if (e is null) return NotFound();
            return new PaveDto
            {
                SessionId = e.SessionId,
                PilotReadinessNotes = e.PilotReadinessNotes,
                PilotRating = e.PilotRating,
                AircraftConditionNotes = e.AircraftConditionNotes,
                AircraftRating = e.AircraftRating,
                EnvironmentStatusNotes = e.EnvironmentStatusNotes,
                EnvironmentRating = e.EnvironmentRating,
                ExternalPressuresNotes = e.ExternalPressuresNotes,
                ExternalPressuresRating = e.ExternalPressuresRating
            };
        }
    }

    // ══════════════════════════════════════════════════════════
    //  DECIDE Controller
    // ══════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/[controller]")]
    public class DecideController : ControllerBase
    {
        private readonly AdmDbContext _db;
        public DecideController(AdmDbContext db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<DecideReportDto>> Submit([FromBody] DecideDto dto)
        {
            if (dto.ConsiderOptions.Count < 3)
                return BadRequest("The DECIDE model requires at least 3 options in the Consider step.");

            var session = await _db.Sessions
                .Include(s => s.DecideModel)
                .FirstOrDefaultAsync(s => s.Id == dto.SessionId);
            if (session is null) return NotFound("Session not found.");

            if (session.DecideModel is not null)
                _db.DecideModels.Remove(session.DecideModel);

            var riskScore = RiskService.ComputeRiskScore(dto.EvaluateSeverity, dto.EvaluateLikelihood);

            var entity = new DecideModel
            {
                SessionId = dto.SessionId,
                DetectNotes = dto.DetectNotes,
                DetectRating = dto.DetectRating,
                EvaluateSeverity = dto.EvaluateSeverity,
                EvaluateLikelihood = dto.EvaluateLikelihood,
                EvaluateRiskScore = riskScore,
                ConsiderOptions = RiskService.SerializeOptions(dto.ConsiderOptions),
                IntegrateNotes = dto.IntegrateNotes,
                DecideAction = dto.DecideAction,
                ExecuteNotes = dto.ExecuteNotes,
                ReassessNotes = dto.ReassessNotes
            };
            _db.DecideModels.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(new DecideReportDto
            {
                SessionId = dto.SessionId,
                DetectNotes = dto.DetectNotes,
                DetectRating = dto.DetectRating,
                EvaluateSeverity = dto.EvaluateSeverity,
                EvaluateLikelihood = dto.EvaluateLikelihood,
                EvaluateRiskScore = riskScore,
                ConsiderOptions = dto.ConsiderOptions,
                IntegrateNotes = dto.IntegrateNotes,
                DecideAction = dto.DecideAction,
                ExecuteNotes = dto.ExecuteNotes,
                ReassessNotes = dto.ReassessNotes
            });
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<DecideReportDto>> Get(int sessionId)
        {
            var e = await _db.DecideModels.FirstOrDefaultAsync(d => d.SessionId == sessionId);
            if (e is null) return NotFound();
            return new DecideReportDto
            {
                SessionId = e.SessionId,
                DetectNotes = e.DetectNotes,
                DetectRating = e.DetectRating,
                EvaluateSeverity = e.EvaluateSeverity,
                EvaluateLikelihood = e.EvaluateLikelihood,
                EvaluateRiskScore = e.EvaluateRiskScore,
                ConsiderOptions = RiskService.DeserializeOptions(e.ConsiderOptions),
                IntegrateNotes = e.IntegrateNotes,
                DecideAction = e.DecideAction,
                ExecuteNotes = e.ExecuteNotes,
                ReassessNotes = e.ReassessNotes
            };
        }
    }

    // ══════════════════════════════════════════════════════════
    //  Final Decision Controller
    // ══════════════════════════════════════════════════════════
    [ApiController]
    [Route("api/[controller]")]
    public class FinalDecisionController : ControllerBase
    {
        private readonly AdmDbContext _db;
        public FinalDecisionController(AdmDbContext db) => _db = db;

        // POST /api/finaldecision/{sessionId} — Compute and store final Go/No-Go
        [HttpPost("{sessionId}")]
        public async Task<ActionResult<FinalDecisionDto>> Compute(int sessionId)
        {
            var session = await _db.Sessions
                .Include(s => s.ImsafeChecklist)
                .Include(s => s.PaveChecklist)
                .Include(s => s.DecideModel)
                .Include(s => s.FinalDecision)
                .FirstOrDefaultAsync(s => s.Id == sessionId);

            if (session is null) return NotFound("Session not found.");
            if (session.ImsafeChecklist is null) return BadRequest("IMSAFE checklist not completed.");
            if (session.PaveChecklist is null) return BadRequest("PAVE checklist not completed.");
            if (session.DecideModel is null) return BadRequest("DECIDE model not completed.");

            if (session.FinalDecision is not null)
                _db.FinalDecisions.Remove(session.FinalDecision);

            var (overall, goNoGo, notes) = RiskService.ComputeFinalDecision(
                session.ImsafeChecklist,
                session.PaveChecklist,
                session.DecideModel);

            var entity = new FinalDecision
            {
                SessionId = sessionId,
                OverallRisk = overall,
                GoNoGo = goNoGo,
                RecommendationNotes = notes
            };
            _db.FinalDecisions.Add(entity);
            await _db.SaveChangesAsync();

            return Ok(new FinalDecisionDto
            {
                SessionId = sessionId,
                OverallRisk = overall,
                GoNoGo = goNoGo,
                RecommendationNotes = notes,
                CreatedAt = entity.CreatedAt
            });
        }

        [HttpGet("{sessionId}")]
        public async Task<ActionResult<FinalDecisionDto>> Get(int sessionId)
        {
            var e = await _db.FinalDecisions.FirstOrDefaultAsync(f => f.SessionId == sessionId);
            if (e is null) return NotFound();
            return new FinalDecisionDto
            {
                SessionId = e.SessionId,
                OverallRisk = e.OverallRisk,
                GoNoGo = e.GoNoGo,
                RecommendationNotes = e.RecommendationNotes,
                CreatedAt = e.CreatedAt
            };
        }
    }
}
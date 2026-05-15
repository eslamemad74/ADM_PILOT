using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdmWorksheet.API.Data;
using AdmWorksheet.API.Models;
using AdmWorksheet.API.DTOs;
using AdmWorksheet.API.Services;

namespace AdmWorksheet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        private readonly AdmDbContext _db;
        public SessionsController(AdmDbContext db) => _db = db;

        // POST /api/sessions — Start a new ADM session for a pilot
        [HttpPost]
        public async Task<ActionResult<SessionResponseDto>> Create([FromBody] SessionCreateDto dto)
        {
            var pilot = await _db.Pilots.FindAsync(dto.PilotId);
            if (pilot is null) return NotFound("Pilot not found.");

            var session = new Session
            {
                PilotId = dto.PilotId,
                FlightDate = dto.FlightDate
            };
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = session.Id }, await BuildDto(session, pilot));
        }

        // GET /api/sessions — List all sessions
        [HttpGet]
        public async Task<ActionResult<List<SessionResponseDto>>> GetAll()
        {
            var sessions = await _db.Sessions
                .Include(s => s.Pilot)
                .Include(s => s.ImsafeChecklist)
                .Include(s => s.PaveChecklist)
                .Include(s => s.DecideModel)
                .Include(s => s.FinalDecision)
                .ToListAsync();
            return sessions.Select(s => BuildDtoSync(s)).ToList();
        }

        // GET /api/sessions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionResponseDto>> GetById(int id)
        {
            var session = await _db.Sessions
                .Include(s => s.Pilot)
                .Include(s => s.ImsafeChecklist)
                .Include(s => s.PaveChecklist)
                .Include(s => s.DecideModel)
                .Include(s => s.FinalDecision)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (session is null) return NotFound();
            return BuildDtoSync(session);
        }

        // GET /api/sessions/{id}/report — Full report for a session
        [HttpGet("{id}/report")]
        public async Task<ActionResult<SessionReportDto>> GetReport(int id)
        {
            var session = await _db.Sessions
                .Include(s => s.Pilot)
                .Include(s => s.ImsafeChecklist)
                .Include(s => s.PaveChecklist)
                .Include(s => s.DecideModel)
                .Include(s => s.FinalDecision)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (session is null) return NotFound();
            if (session.Pilot is null) return NotFound("Pilot data missing.");

            var report = new SessionReportDto
            {
                Session = BuildDtoSync(session),
                Pilot = new PilotResponseDto
                {
                    Id = session.Pilot.Id,
                    Name = session.Pilot.Name,
                    Age = session.Pilot.Age,
                    LicenseNumber = session.Pilot.LicenseNumber,
                    TotalFlightHours = session.Pilot.TotalFlightHours,
                    CreatedAt = session.Pilot.CreatedAt
                }
            };

            if (session.ImsafeChecklist is not null)
            {
                var i = session.ImsafeChecklist;
                report.Imsafe = new ImsafeDto
                {
                    SessionId = i.SessionId,
                    IllnessRating = i.IllnessRating,
                    IllnessNotes = i.IllnessNotes,
                    MedicationRating = i.MedicationRating,
                    MedicationNotes = i.MedicationNotes,
                    StressRating = i.StressRating,
                    StressNotes = i.StressNotes,
                    AlcoholRating = i.AlcoholRating,
                    AlcoholNotes = i.AlcoholNotes,
                    FatigueRating = i.FatigueRating,
                    FatigueNotes = i.FatigueNotes,
                    EmotionRating = i.EmotionRating,
                    EmotionNotes = i.EmotionNotes
                };
            }

            if (session.PaveChecklist is not null)
            {
                var p = session.PaveChecklist;
                report.Pave = new PaveDto
                {
                    SessionId = p.SessionId,
                    PilotReadinessNotes = p.PilotReadinessNotes,
                    PilotRating = p.PilotRating,
                    AircraftConditionNotes = p.AircraftConditionNotes,
                    AircraftRating = p.AircraftRating,
                    EnvironmentStatusNotes = p.EnvironmentStatusNotes,
                    EnvironmentRating = p.EnvironmentRating,
                    ExternalPressuresNotes = p.ExternalPressuresNotes,
                    ExternalPressuresRating = p.ExternalPressuresRating
                };
            }

            if (session.DecideModel is not null)
            {
                var d = session.DecideModel;
                report.Decide = new DecideReportDto
                {
                    SessionId = d.SessionId,
                    DetectNotes = d.DetectNotes,
                    DetectRating = d.DetectRating,
                    EvaluateSeverity = d.EvaluateSeverity,
                    EvaluateLikelihood = d.EvaluateLikelihood,
                    EvaluateRiskScore = d.EvaluateRiskScore,
                    ConsiderOptions = RiskService.DeserializeOptions(d.ConsiderOptions),
                    IntegrateNotes = d.IntegrateNotes,
                    DecideAction = d.DecideAction,
                    ExecuteNotes = d.ExecuteNotes,
                    ReassessNotes = d.ReassessNotes
                };
            }

            if (session.FinalDecision is not null)
            {
                var f = session.FinalDecision;
                report.FinalDecision = new FinalDecisionDto
                {
                    SessionId = f.SessionId,
                    OverallRisk = f.OverallRisk,
                    GoNoGo = f.GoNoGo,
                    RecommendationNotes = f.RecommendationNotes,
                    CreatedAt = f.CreatedAt
                };
            }

            return report;
        }

        private static SessionResponseDto BuildDtoSync(Session s) => new()
        {
            Id = s.Id,
            PilotId = s.PilotId,
            PilotName = s.Pilot?.Name ?? "",
            FlightDate = s.FlightDate,
            CreatedAt = s.CreatedAt,
            HasImsafe = s.ImsafeChecklist is not null,
            HasPave = s.PaveChecklist is not null,
            HasDecide = s.DecideModel is not null,
            HasFinalDecision = s.FinalDecision is not null
        };

        private static async Task<SessionResponseDto> BuildDto(Session s, Pilot p)
        {
            await Task.CompletedTask;
            return new SessionResponseDto
            {
                Id = s.Id,
                PilotId = s.PilotId,
                PilotName = p.Name,
                FlightDate = s.FlightDate,
                CreatedAt = s.CreatedAt
            };
        }
    }
}
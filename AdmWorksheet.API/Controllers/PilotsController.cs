using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdmWorksheet.API.Data;
using AdmWorksheet.API.Models;
using AdmWorksheet.API.DTOs;

namespace AdmWorksheet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PilotsController : ControllerBase
    {
        private readonly AdmDbContext _db;
        public PilotsController(AdmDbContext db) => _db = db;

        // POST /api/pilots — Register a new pilot
        [HttpPost]
        public async Task<ActionResult<PilotResponseDto>> Create([FromBody] PilotCreateDto dto)
        {
            var pilot = new Pilot
            {
                Name = dto.Name,
                Age = dto.Age,
                LicenseNumber = dto.LicenseNumber,
                TotalFlightHours = dto.TotalFlightHours
            };
            _db.Pilots.Add(pilot);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = pilot.Id }, ToDto(pilot));
        }

        // GET /api/pilots — List all pilots
        [HttpGet]
        public async Task<ActionResult<List<PilotResponseDto>>> GetAll()
        {
            var pilots = await _db.Pilots.ToListAsync();
            return pilots.Select(ToDto).ToList();
        }

        // GET /api/pilots/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PilotResponseDto>> GetById(int id)
        {
            var pilot = await _db.Pilots.FindAsync(id);
            if (pilot is null) return NotFound();
            return ToDto(pilot);
        }

        private static PilotResponseDto ToDto(Pilot p) => new()
        {
            Id = p.Id,
            Name = p.Name,
            Age = p.Age,
            LicenseNumber = p.LicenseNumber,
            TotalFlightHours = p.TotalFlightHours,
            CreatedAt = p.CreatedAt
        };
    }
}
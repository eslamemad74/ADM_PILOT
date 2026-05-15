using Microsoft.EntityFrameworkCore;
using AdmWorksheet.API.Models;

namespace AdmWorksheet.API.Data
{
    public class AdmDbContext : DbContext
    {
        public AdmDbContext(DbContextOptions<AdmDbContext> options) : base(options) { }

        public DbSet<Pilot> Pilots => Set<Pilot>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<ImsafeChecklist> ImsafeChecklists => Set<ImsafeChecklist>();
        public DbSet<PaveChecklist> PaveChecklists => Set<PaveChecklist>();
        public DbSet<DecideModel> DecideModels => Set<DecideModel>();
        public DbSet<FinalDecision> FinalDecisions => Set<FinalDecision>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Session → Pilot (Many-to-One)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Pilot)
                .WithMany(p => p.Sessions)
                .HasForeignKey(s => s.PilotId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session → ImsafeChecklist (One-to-One)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.ImsafeChecklist)
                .WithOne(i => i.Session)
                .HasForeignKey<ImsafeChecklist>(i => i.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session → PaveChecklist (One-to-One)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.PaveChecklist)
                .WithOne(p => p.Session)
                .HasForeignKey<PaveChecklist>(p => p.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session → DecideModel (One-to-One)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.DecideModel)
                .WithOne(d => d.Session)
                .HasForeignKey<DecideModel>(d => d.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session → FinalDecision (One-to-One)
            modelBuilder.Entity<Session>()
                .HasOne(s => s.FinalDecision)
                .WithOne(f => f.Session)
                .HasForeignKey<FinalDecision>(f => f.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Store enums as strings
            modelBuilder.Entity<ImsafeChecklist>()
                .Property(i => i.IllnessRating).HasConversion<string>();
            modelBuilder.Entity<ImsafeChecklist>()
                .Property(i => i.MedicationRating).HasConversion<string>();
            modelBuilder.Entity<ImsafeChecklist>()
                .Property(i => i.StressRating).HasConversion<string>();
            modelBuilder.Entity<ImsafeChecklist>()
                .Property(i => i.AlcoholRating).HasConversion<string>();
            modelBuilder.Entity<ImsafeChecklist>()
                .Property(i => i.FatigueRating).HasConversion<string>();
            modelBuilder.Entity<ImsafeChecklist>()
                .Property(i => i.EmotionRating).HasConversion<string>();

            modelBuilder.Entity<PaveChecklist>()
                .Property(p => p.PilotRating).HasConversion<string>();
            modelBuilder.Entity<PaveChecklist>()
                .Property(p => p.AircraftRating).HasConversion<string>();
            modelBuilder.Entity<PaveChecklist>()
                .Property(p => p.EnvironmentRating).HasConversion<string>();
            modelBuilder.Entity<PaveChecklist>()
                .Property(p => p.ExternalPressuresRating).HasConversion<string>();

            modelBuilder.Entity<DecideModel>()
                .Property(d => d.DetectRating).HasConversion<string>();
            modelBuilder.Entity<DecideModel>()
                .Property(d => d.EvaluateSeverity).HasConversion<string>();
            modelBuilder.Entity<DecideModel>()
                .Property(d => d.EvaluateLikelihood).HasConversion<string>();
            modelBuilder.Entity<DecideModel>()
                .Property(d => d.EvaluateRiskScore).HasConversion<string>();

            modelBuilder.Entity<FinalDecision>()
                .Property(f => f.OverallRisk).HasConversion<string>();
            modelBuilder.Entity<FinalDecision>()
                .Property(f => f.GoNoGo).HasConversion<string>();
        }
    }
}
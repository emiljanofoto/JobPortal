using Microsoft.EntityFrameworkCore;
using JobBoardApi.Models;

namespace JobBoardApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<JobApplication> JobApplications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Role).IsRequired();
            });

            // Configure Job entity
            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.ExpirationDate).IsRequired();
            });

            // Configure JobApplication entity
            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).IsRequired();

                // Configure foreign key relationships
                entity.HasOne(e => e.Job)
                      .WithMany(j => j.Applications)
                      .HasForeignKey(e => e.JobId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Candidate)
                      .WithMany(u => u.Applications)
                      .HasForeignKey(e => e.CandidateId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Ensure one application per candidate per job
                entity.HasIndex(e => new { e.JobId, e.CandidateId }).IsUnique();
            });
        }
    }
}
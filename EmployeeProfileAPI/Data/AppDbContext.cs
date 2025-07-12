using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.Models.AuthModels;
using EmployeeProfileAPI.Models.UserMoreDetailModels;
using EmployeeProfileAPI.Data;

namespace EmployeeProfileAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === DB Sets ===
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<WorkforceIssue> WorkforceIssues { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<HRUser> HRUsers { get; set; }
        public DbSet<WorkforceUser> WorkforceUsers { get; set; }
        public DbSet<PasswordChangeVerification> PasswordChangeVerifications { get; set; }

        public DbSet<HRProfile> HRProfiles { get; set; }
        public DbSet<WorkforceProfile> WorkforceProfiles { get; set; }
        public DbSet<SmtpSettings> SmtpSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === Configurations ===

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeID);
                entity.Property(e => e.EmployeeID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.HasKey(s => s.SkillID);
                entity.Property(s => s.SkillID).ValueGeneratedOnAdd();
                entity.HasOne(s => s.Employee)
                      .WithMany(e => e.Skills)
                      .HasForeignKey(s => s.EmployeeID);
            });

            modelBuilder.Entity<Education>(entity =>
            {
                entity.HasKey(e => e.EducationID);
                entity.Property(e => e.EducationID).ValueGeneratedOnAdd();
                entity.HasOne(ed => ed.Employee)
                      .WithMany(e => e.Education)
                      .HasForeignKey(ed => ed.EmployeeID);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.ProjectID);
                entity.Property(p => p.ProjectID).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<ProjectAssignment>(entity =>
            {
                entity.HasKey(pa => pa.AssignmentID);
                entity.Property(pa => pa.AssignmentID).ValueGeneratedOnAdd();

                entity.HasOne(pa => pa.Project)
                      .WithMany(p => p.ProjectAssignments)
                      .HasForeignKey(pa => pa.ProjectID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pa => pa.Employee)
                      .WithMany(e => e.ProjectAssignments)
                      .HasForeignKey(pa => pa.EmployeeID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(pa => new { pa.ProjectID, pa.EmployeeID }).IsUnique();
            });

            modelBuilder.Entity<EmployeeLeave>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.Property(e => e.EmployeeId).HasMaxLength(450);
            });

            modelBuilder.Entity<WorkforceIssue>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasKey(lr => lr.Id);
                entity.Property(lr => lr.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("SystemUsers");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PasswordChangeVerification>(entity =>
            {
                entity.HasKey(v => v.VerificationId);
                entity.Property(v => v.VerificationId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(up => up.EmployeeID);
                // No ValueGeneratedOnAdd since it's a string key (manually assigned)
            });

            // Add keys for new models if not already decorated in model files
            modelBuilder.Entity<Admin>().HasKey(a => a.Id);
            modelBuilder.Entity<HRUser>().HasKey(h => h.Id);
            modelBuilder.Entity<WorkforceUser>().HasKey(w => w.Id);
            modelBuilder.Entity<HRProfile>().HasKey(hp => hp.Id);
            modelBuilder.Entity<WorkforceProfile>().HasKey(wp => wp.Id);
            modelBuilder.Entity<SmtpSettings>().HasKey(s => s.Id);
        }
    }
}

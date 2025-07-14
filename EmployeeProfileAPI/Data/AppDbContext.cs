using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.Models.AuthModels;
using EmployeeProfileAPI.Models.UserMoreDetailModels;

namespace EmployeeProfileAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // --- CORRECT DBSET PROPERTIES ---

        // Main Employee Profile System
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }

        // Other application models
        public DbSet<WorkforceIssue> WorkforceIssues { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        
        // Auth and User system models
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<HRUser> HRUsers { get; set; }
        public DbSet<WorkforceUser> WorkforceUsers { get; set; }
        public DbSet<PasswordChangeVerification> PasswordChangeVerifications { get; set; }
        
        // User Profile extension models
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<HRProfile> HRProfiles { get; set; }
        public DbSet<WorkforceProfile> WorkforceProfiles { get; set; }
        public DbSet<SmtpSettings> SmtpSettings { get; set; }

        // NOTE: The incorrect DbSets for EmployeeProfile, ProfileSkill, and ProfileEducation have been removed.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CORRECTED AND VERIFIED CONFIGURATIONS ---

            // Employee Entity
            modelBuilder.Entity<Employee>(entity => 
            { 
                entity.HasKey(e => e.EmployeeID); 
                entity.Property(e => e.EmployeeID).ValueGeneratedOnAdd(); 
            });

            // Skill Entity with Cascade Delete
            modelBuilder.Entity<Skill>(entity => 
            { 
                entity.HasKey(s => s.SkillID); 
                entity.Property(s => s.SkillID).ValueGeneratedOnAdd(); 
                
                // Defines the relationship: One Employee has Many Skills
                entity.HasOne(s => s.Employee)
                      .WithMany(e => e.Skills) // The navigation property in the Employee class
                      .HasForeignKey(s => s.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade); // **CRITICAL: If an Employee is deleted, delete their Skills**
            });

            // Education Entity with Cascade Delete
            modelBuilder.Entity<Education>(entity => 
            { 
                entity.HasKey(e => e.EducationID); 
                entity.Property(e => e.EducationID).ValueGeneratedOnAdd(); 

                // Defines the relationship: One Employee has Many Education records
                entity.HasOne(ed => ed.Employee)
                      .WithMany(e => e.Education) // The navigation property in the Employee class
                      .HasForeignKey(ed => ed.EmployeeID)
                      .OnDelete(DeleteBehavior.Cascade); // **CRITICAL: If an Employee is deleted, delete their Education records**
            });
            
            // --- Other existing configurations (unchanged) ---
            modelBuilder.Entity<Project>(entity => { entity.HasKey(p => p.ProjectID); entity.Property(p => p.ProjectID).ValueGeneratedOnAdd(); });
            modelBuilder.Entity<ProjectAssignment>(entity => { entity.HasKey(pa => pa.AssignmentID); entity.Property(pa => pa.AssignmentID).ValueGeneratedOnAdd(); entity.HasOne(pa => pa.Project).WithMany(p => p.ProjectAssignments).HasForeignKey(pa => pa.ProjectID).OnDelete(DeleteBehavior.Restrict); entity.HasOne(pa => pa.Employee).WithMany(e => e.ProjectAssignments).HasForeignKey(pa => pa.EmployeeID).OnDelete(DeleteBehavior.Restrict); entity.HasIndex(pa => new { pa.ProjectID, pa.EmployeeID }).IsUnique(); });
            modelBuilder.Entity<EmployeeLeave>(entity => { entity.HasKey(e => e.EmployeeId); entity.Property(e => e.EmployeeId).HasMaxLength(450); });
            modelBuilder.Entity<WorkforceIssue>(entity => { entity.HasKey(i => i.Id); entity.Property(i => i.Id).ValueGeneratedOnAdd(); });
            modelBuilder.Entity<LeaveRequest>(entity => { entity.HasKey(lr => lr.Id); entity.Property(lr => lr.Id).ValueGeneratedOnAdd(); });
            modelBuilder.Entity<Announcement>(entity => { entity.HasKey(a => a.Id); entity.Property(a => a.Id).ValueGeneratedOnAdd(); });
            modelBuilder.Entity<PasswordChangeVerification>(entity => { entity.HasKey(v => v.VerificationId); entity.Property(v => v.VerificationId).ValueGeneratedOnAdd(); });
            modelBuilder.Entity<UserProfile>(entity => { entity.HasKey(up => up.EmployeeID); });
            modelBuilder.Entity<Admin>().HasKey(a => a.Id);
            modelBuilder.Entity<HRUser>().HasKey(h => h.Id);
            modelBuilder.Entity<WorkforceUser>().HasKey(w => w.Id);
            modelBuilder.Entity<HRProfile>().HasKey(hp => hp.Id);
            modelBuilder.Entity<WorkforceProfile>().HasKey(wp => wp.Id);
            modelBuilder.Entity<SmtpSettings>().HasKey(s => s.Id);
            
            // NOTE: The incorrect configurations for EmployeeProfile, ProfileSkill, and ProfileEducation have been removed.
        }
    }
}
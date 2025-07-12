// File: Data/AppDbContext.cs

using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Models; // General models
using HRWorkForceSystemBackend.Models.AuthModels;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;
using HRWorkForceSystemBackend.Models.WorkforceModels;
using HRWorkForceSystemBackend.Models.SkillgapModels;
using HRWorkForceSystemBackend.Models.ProjectsModels;
using System;

namespace HRWorkForceSystemBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Your existing DbSets
        public DbSet<Admin> Admins { get; set; }
        public DbSet<HRUser> HRUsers { get; set; }
        public DbSet<WorkforceUser> WorkforceUsers { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<TrainingProgram> TrainingPrograms { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Attrition> Attritions { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectSkillRequirement> ProjectSkillRequirements { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the keyless entity type for Summary
            modelBuilder.Entity<Summary>().HasNoKey();

            // --- CORRECTED ANNOUNCEMENT CONFIGURATION ---
            // Define the relationship for the Admin sender
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.AdminSender)
                .WithMany() // An Admin can send many announcements
                .HasForeignKey(a => a.AdminSenderId)
                .IsRequired(false) // The key is nullable
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a sender if they have announcements

            // Define the relationship for the HRUser sender
            modelBuilder.Entity<Announcement>()
                .HasOne(a => a.HRUserSender)
                .WithMany() // An HRUser can send many announcements
                .HasForeignKey(a => a.HRUserSenderId)
                .IsRequired(false) // The key is nullable
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
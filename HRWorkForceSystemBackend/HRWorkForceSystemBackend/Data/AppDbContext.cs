using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Models.AuthModels;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;
using HRWorkForceSystemBackend.Models.FeedbackModels;
using HRWorkForceSystemBackend.Models.UserMoreDetailModels;
using HRWorkForceSystemBackend.Models.SkillsModels;
using HRWorkForceSystemBackend.Models.ProjectModels;
using HRWorkForceSystemBackend.Models.LeaveModels;
using HRWorkForceSystemBackend.Models.WorkforceModels;
using HRWorkForceSystemBackend.Controllers;
using HRWorkForceSystemBackend.Models;






namespace HRWorkForceSystemBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<HRUser> HRUsers { get; set; }

        public DbSet<WorkforceUser> WorkforceUsers { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<TrainingProgram> TrainingPrograms { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }

        //public DbSet<Promotion> Promotions { get; set; }

        public DbSet<WorkforceProfile> WorkforceProfiles { get; set; }

        public DbSet<HRProfile> HRProfiles { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectAssignment> ProjectAssignments { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        //public DbSet<HRProfile> HRProfiles { get; set; }
        public DbSet<Movement> Movements { get; set; }

        public DbSet<Attrition> Attritions { get; set; }

        public DbSet<TrainingAssignment> TrainingAssignments { get; set; }

        public DbSet<Announcement> Announcements { get; set; }

        //public DbSet<Message> Messages { get; set; }








        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    // Ensure WorkforceId is unique in WorkforceUser
        //    modelBuilder.Entity<WorkforceUser>()
        //        .HasIndex(w => w.WorkforceId)
        //        .IsUnique();


        //}




    }
}

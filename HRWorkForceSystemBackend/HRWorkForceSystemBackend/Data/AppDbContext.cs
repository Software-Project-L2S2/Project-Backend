using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Models.AuthModels;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;
using HRWorkForceSystemBackend.Models.FeedbackModels;
using HRWorkForceSystemBackend.Models.PromotionModels;
using HRWorkForceSystemBackend.Models.UserMoreDetailModels;


namespace HRWorkForceSystemBackend.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<HRUser> HRUsers { get; set; }

        public DbSet<WorkforceUser> WorkforceUsers { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<TrainingProgram> TrainingPrograms { get; set; }

        public DbSet<Feedback>Feedbacks { get; set; }

        //public DbSet<Promotion> Promotions { get; set; }

        public DbSet<WorkforceProfile> WorkforceProfiles{ get; set; }

        public DbSet<HRProfile> HRProfiles { get; set; }

        //public DbSet<HRProfile> HRProfiles { get; set; }


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

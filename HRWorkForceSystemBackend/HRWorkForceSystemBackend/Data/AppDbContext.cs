using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;
using HRWorkForceSystemBackend.Models.AuthModels;
using HRWorkForceSystemBackend.Models.LeaveModels;

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

        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        





    }
}
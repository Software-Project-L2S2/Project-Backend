using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.DTOs.SkillgapDTOs; // This DTO folder should be created
using HRWorkForceSystemBackend.Models.ProjectModels;
using HRWorkForceSystemBackend.Models.SkillsModels;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillGapController : ControllerBase
    {
        private readonly AppDbContext _context;

        // CORRECTED CONSTRUCTOR
        public SkillGapController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SkillGap/project-summary
        [HttpGet("project-summary")]
        public async Task<ActionResult<IEnumerable<SkillGapProjectSummaryDto>>> GetProjectSkillSummary()
        {
            // 1. Get all projects and their current assignments
            var projects = await _context.Projects
                .AsNoTracking()
                .Include(p => p.ProjectAssignments)
                .ToListAsync();

            // 2. Get all employee skills from the database in one efficient query
            var allEmployeeSkills = await _context.Skills
                .AsNoTracking()
                .Select(s => new { s.EmployeeID, s.SkillName, s.Level })
                .ToListAsync();

            var projectSummaries = new List<SkillGapProjectSummaryDto>();

            foreach (var project in projects)
            {
                var summary = new SkillGapProjectSummaryDto
                {
                    ProjectId = project.ProjectID,
                    ProjectName = project.Name,
                    EmployeesNeeded = project.EmployeeCount,
                    EmployeesAssigned = project.ProjectAssignments.Count
                };

                // 3. Parse the required skills string from the project model
                var requiredSkills = ParseSkills(project.Skills);
                summary.RequiredSkills = requiredSkills.Select(s => $"{s.Name} (Level {s.Level})").ToList();

                var employeesAvailableForProject = new HashSet<int>();
                
                foreach (var reqSkill in requiredSkills)
                {
                    // Find all employees who have the required skill at or above the required level
                    var qualifiedEmployees = allEmployeeSkills
                        .Where(empSkill => empSkill.SkillName.Equals(reqSkill.Name, StringComparison.OrdinalIgnoreCase) 
                                        && empSkill.Level >= reqSkill.Level)
                        .Select(empSkill => empSkill.EmployeeID)
                        .ToList();
                    
                    // Add qualified employees to a HashSet to count unique people
                    foreach(var empId in qualifiedEmployees)
                    {
                        employeesAvailableForProject.Add(empId);
                    }

                    // A deficit exists if we can't find anyone with the required skill at the required level
                    if (!qualifiedEmployees.Any())
                    {
                        summary.SkillDeficits.Add($"No one available with skill: {reqSkill.Name} at Level {reqSkill.Level}");
                    }
                }

                summary.EmployeesAvailable = employeesAvailableForProject.Count;

                // 4. Determine the overall status of the project
                if (summary.EmployeesAssigned >= summary.EmployeesNeeded)
                {
                    summary.Status = "Good"; // Fully staffed
                }
                else if (summary.EmployeesAvailable >= summary.EmployeesNeeded)
                {
                    summary.Status = "Sufficient"; // Enough people exist, they just need to be assigned
                }
                else if (summary.EmployeesAvailable > summary.EmployeesAssigned)
                {
                    summary.Status = "Low"; // Some people are available, but not enough to fully staff the project
                }
                else
                {
                    summary.Status = "Critical"; // Not enough skilled people exist in the entire company
                }

                projectSummaries.Add(summary);
            }

            return Ok(projectSummaries);
        }

        // GET: api/SkillGap/employee-summary
        [HttpGet("employee-summary")]
        public async Task<ActionResult<IEnumerable<SkillGapEmployeeSummaryDto>>> GetEmployeeSkillSummary()
        {
            var employeeSummaries = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Skills)
                .Include(e => e.ProjectAssignments)
                    .ThenInclude(pa => pa.Project)
                .Select(e => new SkillGapEmployeeSummaryDto
                {
                    EmployeeId = e.EmployeeID,
                    EmployeeName = e.Name,
                    Designation = e.Designation,
                    Skills = e.Skills.Select(s => $"{s.SkillName} (Level {s.Level})").ToList(),
                    AssignedProjects = e.ProjectAssignments.Select(pa => pa.Project.Name).ToList()
                }).ToListAsync();

            return Ok(employeeSummaries);
        }

        // Private helper method to parse the skill string
        private List<(string Name, int Level)> ParseSkills(string skillsString)
        {
            var parsedSkills = new List<(string, int)>();
            if (string.IsNullOrWhiteSpace(skillsString)) return parsedSkills;

            var skills = skillsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var skill in skills)
            {
                var parts = skill.Split(new[] { "(Level" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    var name = parts[0].Trim();
                    if (int.TryParse(parts[1].Trim().Replace(")", ""), out int level))
                    {
                        parsedSkills.Add((name, level));
                    }
                }
            }
            return parsedSkills;
        }
    }
}
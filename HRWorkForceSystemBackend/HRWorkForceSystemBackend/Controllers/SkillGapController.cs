using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.DTOs.SkillgapDTOs;
using HRWorkForceSystemBackend.DTOs.ProjectsDTOs;
using HRWorkForceSystemBackend.Models.SkillgapModels;
using HRWorkForceSystemBackend.Models.ProjectsModels;
using HRWorkForceSystemBackend.Models.AuthModels;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillGapController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillGapController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SkillGap/project-summary
        [HttpGet("project-summary")]
        public async Task<ActionResult<IEnumerable<SkillGapProjectSummaryDto>>> GetProjectSkillSummary()
        {
            var projects = await _context.Projects
                                         .AsNoTracking()
                                         .Where(p => p.IsActive)
                                         .Include(p => p.ProjectSkillRequirements)
                                             .ThenInclude(psr => psr.Skill)
                                         .ToListAsync();

            var allEmployeeSkills = await _context.EmployeeSkills
                                                  .AsNoTracking()
                                                  .ToListAsync();

            var projectSummaries = new List<SkillGapProjectSummaryDto>();

            foreach (var project in projects)
            {
                var dto = new SkillGapProjectSummaryDto
                {
                    ProjectId = project.Id,
                    ProjectName = project.ProjectName,
                    RequiredSkills = project.ProjectSkillRequirements
                                            .Select(psr => new SkillDto
                                            {
                                                Id = psr.Skill.Id,
                                                SkillName = psr.Skill?.SkillName ?? "Unknown"
                                            })
                                            .ToList()
                };

                var uniqueEmployeesForProject = allEmployeeSkills
                    .Where(es => project.ProjectSkillRequirements.Any(psr =>
                        psr.SkillId == es.SkillId && es.ProficiencyLevel >= psr.RequiredProficiencyLevel))
                    .Select(es => es.EmployeeId)
                    .Distinct()
                    .Count();
                dto.AvailableEmployees = uniqueEmployeesForProject;

                foreach (var requiredSkill in project.ProjectSkillRequirements)
                {
                    var skillName = requiredSkill.Skill.SkillName;
                    var requiredLevel = requiredSkill.RequiredProficiencyLevel;
                    var neededCount = requiredSkill.NumberOfResourcesNeeded;

                    var skilledEmployeesCount = allEmployeeSkills
                        .Count(es => es.SkillId == requiredSkill.SkillId && es.ProficiencyLevel >= requiredLevel);

                    var deficit = neededCount - skilledEmployeesCount;
                    if (deficit > 0)
                    {
                        dto.SkillsNeeded.Add($"{skillName}: Need {deficit}");
                    }
                }

                if (!dto.SkillsNeeded.Any())
                {
                    dto.Status = "Good";
                }
                else
                {
                    bool hasAbsoluteGap = project.ProjectSkillRequirements.Any(psr =>
                        !allEmployeeSkills.Any(e => e.SkillId == psr.SkillId && e.ProficiencyLevel >= psr.RequiredProficiencyLevel));

                    if (dto.AvailableEmployees == 0 || hasAbsoluteGap)
                    {
                        dto.Status = "Critical";
                    }
                    else
                    {
                        dto.Status = "Low";
                    }
                }

                projectSummaries.Add(dto);
            }

            return Ok(projectSummaries);
        }

        // GET: api/SkillGap/employee-summary
        [HttpGet("employee-summary")]
        public async Task<ActionResult<IEnumerable<SkillGapEmployeeSummaryDto>>> GetEmployeeSkillSummary()
        {
            var employeeSummaries = await _context.HRUsers
                .AsNoTracking()
                .Select(e => new SkillGapEmployeeSummaryDto
                {
                    EmployeeId = e.Id,
                    EmployeeName = e.FullName,
                    Role = e.Role,
                    Department = e.Department,
                    SkillsWithProficiency = e.EmployeeSkills
                        .Where(es => es.Skill != null)
                        .Select(es => $"{es.Skill.SkillName}({es.ProficiencyLevel})")
                        .ToList(),
                    LastUpdated = e.EmployeeSkills.Any() ? e.EmployeeSkills.Max(es => es.LastUpdated) : default
                }).ToListAsync();

            return Ok(employeeSummaries);
        }

        // --- POST, PUT, DELETE Methods ---

        [HttpPost("employee-skill")]
        public async Task<ActionResult> AssignEmployeeSkill([FromBody] EmployeeSkillCreationDto employeeSkillDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var employeeExists = await _context.HRUsers.AnyAsync(u => u.Id == employeeSkillDto.EmployeeId);
            if (!employeeExists) return NotFound($"Employee with ID {employeeSkillDto.EmployeeId} not found.");

            var skillExists = await _context.Skills.AnyAsync(s => s.Id == employeeSkillDto.SkillId);
            if (!skillExists) return NotFound($"Skill with ID {employeeSkillDto.SkillId} not found.");

            var existingEmployeeSkill = await _context.EmployeeSkills.FirstOrDefaultAsync(es => es.EmployeeId == employeeSkillDto.EmployeeId && es.SkillId == employeeSkillDto.SkillId);

            if (existingEmployeeSkill != null)
            {
                existingEmployeeSkill.ProficiencyLevel = employeeSkillDto.ProficiencyLevel;
                existingEmployeeSkill.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                var employeeSkill = new EmployeeSkill { EmployeeId = employeeSkillDto.EmployeeId, SkillId = employeeSkillDto.SkillId, ProficiencyLevel = employeeSkillDto.ProficiencyLevel, LastUpdated = DateTime.UtcNow };
                _context.EmployeeSkills.Add(employeeSkill);
            }
            await _context.SaveChangesAsync();
            return Ok("Employee skill assigned/updated successfully.");
        }

        [HttpPost("project-skill-requirement")]
        public async Task<ActionResult> AddProjectSkillRequirement([FromBody] ProjectSkillRequirementCreationDto reqDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var projectExists = await _context.Projects.AnyAsync(p => p.Id == reqDto.ProjectId);
            if (!projectExists) return NotFound($"Project with ID {reqDto.ProjectId} not found.");

            var skillExists = await _context.Skills.AnyAsync(s => s.Id == reqDto.SkillId);
            if (!skillExists) return NotFound($"Skill with ID {reqDto.SkillId} not found.");

            var existingRequirement = await _context.ProjectSkillRequirements.FirstOrDefaultAsync(psr => psr.ProjectId == reqDto.ProjectId && psr.SkillId == reqDto.SkillId);

            if (existingRequirement != null)
            {
                existingRequirement.RequiredProficiencyLevel = reqDto.RequiredProficiencyLevel;
                existingRequirement.NumberOfResourcesNeeded = reqDto.NumberOfResourcesNeeded;
            }
            else
            {
                var newRequirement = new ProjectSkillRequirement { ProjectId = reqDto.ProjectId, SkillId = reqDto.SkillId, RequiredProficiencyLevel = reqDto.RequiredProficiencyLevel, NumberOfResourcesNeeded = reqDto.NumberOfResourcesNeeded };
                _context.ProjectSkillRequirements.Add(newRequirement);
            }
            await _context.SaveChangesAsync();
            return Ok("Project skill requirement added/updated successfully.");
        }

        [HttpPost("add-skill")]
        public async Task<ActionResult<SkillDto>> AddSkill([FromBody] SkillDto skillDto)
        {
            if (string.IsNullOrWhiteSpace(skillDto.SkillName)) return BadRequest("Skill name cannot be empty.");

            var skillNameLower = skillDto.SkillName.ToLower();
            if (await _context.Skills.AnyAsync(s => s.SkillName.ToLower() == skillNameLower))
            {
                return Conflict($"Skill '{skillDto.SkillName}' already exists.");
            }

            var skill = new Skill { SkillName = skillDto.SkillName };
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSkillById), new { id = skill.Id }, new SkillDto { Id = skill.Id, SkillName = skill.SkillName });
        }

        // --- Basic GETTERS for populating dropdowns ---

        [HttpGet("skills/{id}")]
        public async Task<ActionResult<SkillDto>> GetSkillById(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            return new SkillDto { Id = skill.Id, SkillName = skill.SkillName };
        }

        [HttpGet("skills")]
        public async Task<ActionResult<IEnumerable<SkillDto>>> GetAllSkills()
        {
            return await _context.Skills
                .AsNoTracking()
                .Select(s => new SkillDto { Id = s.Id, SkillName = s.SkillName })
                .OrderBy(s => s.SkillName)
                .ToListAsync();
        }

        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllProjects()
        {
            return await _context.Projects
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => new { p.Id, p.ProjectName })
                .OrderBy(p => p.ProjectName)
                .ToListAsync();
        }
    }
}
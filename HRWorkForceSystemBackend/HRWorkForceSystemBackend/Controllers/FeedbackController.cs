using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.FeedbackModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using HRWorkForceSystemBackend.DTOs.FeedbackDTOs;


namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly AppDbContext _context;


        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("submit-feedback")]
        [Authorize(Roles ="Workforce")]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackDto feedbackDto)
        {
            if (string.IsNullOrWhiteSpace(feedbackDto.Emoji) && string.IsNullOrWhiteSpace(feedbackDto.Text))
            {
                return BadRequest("Please provide either emoji or text feedback.");
            }

            var feedback = new Feedback
            {
                Emoji = feedbackDto.Emoji,
                Text = feedbackDto.Text
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok("Thank you for your feedback!");
        }

        [HttpGet("view-feedback")]
        [Authorize(Roles = "Admin,HR,Workforce")]
        public async Task<IActionResult> GetAllFeedback()
        {
            var feedbacks = await _context.Feedbacks.OrderByDescending(f => f.SubmittedAt).ToListAsync();
            return Ok(feedbacks);
        }
    }
}

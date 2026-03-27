using CandidateTest.Api.Data;
using CandidateTest.Api.Models;
using CandidateTest.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CandidateTest.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CandidateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public CandidateController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteCandidate([FromBody] InviteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if user is admin
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user?.Role != "Admin")
                return Forbid();

            // Check if candidate already exists
            var existingCandidate = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingCandidate != null)
                return BadRequest("Candidate with this email already exists");

            // Create candidate user
            var candidate = new User
            {
                Email = request.Email,
                Name = request.Name,
                Role = "Candidate",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()) // Temporary password
            };

            _context.Users.Add(candidate);
            await _context.SaveChangesAsync();

            // Send invitation email
            var subject = "Invitation to Candidate Test";
            var body = $@"
                <h2>Welcome to the Candidate Test Platform</h2>
                <p>Dear {request.Name},</p>
                <p>You have been invited to take a candidate test.</p>
                <p>Please use the following credentials to login:</p>
                <p><strong>Email:</strong> {request.Email}</p>
                <p><strong>Temporary Password:</strong> {candidate.PasswordHash}</p>
                <p>Please change your password after first login.</p>
                <p><a href='{Request.Scheme}://{Request.Host}/Home/Index'>Login Here</a></p>
            ";

            try
            {
                await _emailService.SendEmailAsync(request.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

            return Ok(new { message = "Invitation sent successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCandidates()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user?.Role != "Admin")
                return Forbid();

            var candidates = await _context.Users
                .Where(u => u.Role == "Candidate")
                .Select(u => new { u.Id, u.Name, u.Email })
                .ToListAsync();

            return Ok(candidates);
        }
    }

    public class InviteRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
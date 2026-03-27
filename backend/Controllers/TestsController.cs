using CandidateTest.Api.Data;
using CandidateTest.Api.Models;
using CandidateTest.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CandidateTest.Api.Controllers
{
    [Route("api/tests")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IHubContext<Hubs.LiveHub> _hub;

        public TestsController(ApplicationDbContext db, IScoreCalculator scoreCalculator, IHubContext<Hubs.LiveHub> hub)
        {
            _db = db;
            _scoreCalculator = scoreCalculator;
            _hub = hub;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAvailableTests()
        {
            var tests = await _db.Tests.Include(t => t.Questions).ToListAsync();
            return Ok(tests);
        }

        [HttpGet("questions/{testType}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetQuestionsByType(string testType)
        {
            var questions = await _db.QuestionBanks
                .Where(q => q.TestType == testType)
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.QuestionType,
                    q.TimeLimit
                })
                .ToListAsync();

            return Ok(questions);
        }

        [HttpPost("submit")]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] SubmissionRequest request)
        {
            var test = await _db.Tests.Include(t => t.Questions).FirstOrDefaultAsync(t => t.Id == request.TestId);
            if (test == null) return NotFound();

            var score = _scoreCalculator.Calculate(test, request.Answers);

            var submission = new Submission
            {
                CandidateId = request.CandidateId,
                TestId = request.TestId,
                Answers = request.Answers.Select(a => new AnswerItem
                {
                    QuestionId = a.QuestionId,
                    CandidateAnswer = a.CandidateAnswer,
                    TimeSpentSeconds = a.TimeSpentSeconds,
                    Correct = false
                }).ToList(),
                Score = score,
                SubmittedAt = DateTime.UtcNow
            };

            _db.Submissions.Add(submission);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group($"test-{request.TestId}").SendAsync("CandidateScoreUpdated", new
            {
                submission.Id,
                score,
                candidateId = request.CandidateId
            });

            return Ok(new { submissionId = submission.Id, score });
        }

        [HttpPost]
        [Route("admin/create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestRequest request)
        {
            var test = new Test
            {
                Title = request.Title,
                Description = request.Description,
                TestType = request.TestType,
                Duration = TimeSpan.FromMinutes(request.Duration),
                Questions = request.Questions.Select(q => new Question
                {
                    QuestionBankId = q.QuestionBankId,
                    Type = q.Type,
                    Prompt = q.Prompt,
                    Choices = q.Choices?.ToArray() ?? Array.Empty<string>(),
                    Answer = q.Answer,
                    Points = q.Points
                }).ToList()
            };

            _db.Tests.Add(test);
            await _db.SaveChangesAsync();
            return Ok(test);
        }
    }

    public class SubmissionRequest
    {
        public int CandidateId { get; set; }
        public int TestId { get; set; }
        public List<AnswerRequest> Answers { get; set; } = new List<AnswerRequest>();
    }

    public class AnswerRequest
    {
        public int QuestionId { get; set; }
        public string CandidateAnswer { get; set; } = string.Empty;
        public int TimeSpentSeconds { get; set; }
    }

    public class CreateTestRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public List<CreateQuestion> Questions { get; set; } = new();
    }

    public class CreateQuestion
    {
        public int? QuestionBankId { get; set; }
        public List<string>? Choices { get; set; }
        public string Answer { get; set; } = string.Empty;
        public int Points { get; set; } = 1;
    }
}

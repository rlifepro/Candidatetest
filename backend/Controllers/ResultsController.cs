using CandidateTest.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateTest.Api.Controllers
{
    [Route("api/results")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ResultsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("{testId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetResults(int testId)
        {
            var records = await _db.Submissions
                .Where(s => s.TestId == testId)
                .Include(s => s.Candidate)
                .Include(s => s.Test)
                .ToListAsync();

            var grouped = records.Select(s => new
            {
                s.Id,
                s.CandidateId,
                Candidate = s.Candidate != null ? new { s.Candidate.Id, s.Candidate.Username } : null,
                s.Score,
                s.SubmittedAt,
                s.TestId,
                Test = s.Test != null ? new { s.Test.Id, s.Test.Title } : null
            });

            return Ok(grouped);
        }
    }
}

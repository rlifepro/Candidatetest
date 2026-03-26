using CandidateTest.Api.Data;
using CandidateTest.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateTest.Api.Controllers
{
    [Route("api/questionbank")]
    [ApiController]
    public class QuestionBankController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public QuestionBankController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var bank = await _db.QuestionBank.ToListAsync();
            return Ok(bank);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var question = await _db.QuestionBank.FindAsync(id);
            if (question == null) return NotFound();
            return Ok(question);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] QuestionBank request)
        {
            _db.QuestionBank.Add(request);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = request.Id }, request);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] QuestionBank request)
        {
            var q = await _db.QuestionBank.FindAsync(id);
            if (q == null) return NotFound();

            q.Title = request.Title;
            q.Category = request.Category;
            q.Type = request.Type;
            q.Prompt = request.Prompt;
            q.Choices = request.Choices;
            q.Answer = request.Answer;
            q.Points = request.Points;
            q.SuggestedTimeSeconds = request.SuggestedTimeSeconds;
            q.MaxTimeSeconds = request.MaxTimeSeconds;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var q = await _db.QuestionBank.FindAsync(id);
            if (q == null) return NotFound();
            _db.QuestionBank.Remove(q);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

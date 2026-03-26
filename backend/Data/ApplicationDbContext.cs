using CandidateTest.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CandidateTest.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Test> Tests => Set<Test>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<Submission> Submissions => Set<Submission>();
    }
}

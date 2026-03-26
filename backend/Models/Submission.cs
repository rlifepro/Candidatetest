namespace CandidateTest.Api.Models
{
    public class Submission
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public User? Candidate { get; set; }
        public int TestId { get; set; }
        public Test? Test { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public List<AnswerItem> Answers { get; set; } = new();
        public int Score { get; set; }
    }

    public class AnswerItem
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public Question? Question { get; set; }
        public string CandidateAnswer { get; set; } = string.Empty;
        public bool Correct { get; set; }
        public int TimeSpentSeconds { get; set; } = 0;
    }
}

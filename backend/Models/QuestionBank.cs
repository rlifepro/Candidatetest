namespace CandidateTest.Api.Models
{
    public class QuestionBank
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "MCQ"; // MCQ or Coding
        public string TestType { get; set; } = string.Empty; // e.g., Java, .Net, General
        public string? Options { get; set; } // Pipe-separated for MCQ
        public int? CorrectAnswer { get; set; } // 1-based index for MCQ
        public string? CodeSnippet { get; set; } // For coding questions
        public string? ExpectedOutput { get; set; } // For coding questions
        public int TimeLimit { get; set; } = 60; // seconds
        public List<Question>? Questions { get; set; }
    }
}

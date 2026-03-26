namespace CandidateTest.Api.Models
{
    public class QuestionBank
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = "MCQ";
        public string Prompt { get; set; } = string.Empty;
        public string[] Choices { get; set; } = Array.Empty<string>();
        public string Answer { get; set; } = string.Empty;
        public int Points { get; set; } = 1;
        public int SuggestedTimeSeconds { get; set; } = 60;
        public int MaxTimeSeconds { get; set; } = 300; // hard maximum allowed for this question
        public List<Question>? Questions { get; set; }
    }
}

namespace CandidateTest.Api.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public Test? Test { get; set; }
        public int? QuestionBankId { get; set; }
        public QuestionBank? QuestionBank { get; set; }
        public string Type { get; set; } = "MCQ"; // MCQ or Coding
        public string Prompt { get; set; } = string.Empty;
        public string[] Choices { get; set; } = Array.Empty<string>();
        public string Answer { get; set; } = string.Empty;
        public int Points { get; set; } = 1;
    }
}

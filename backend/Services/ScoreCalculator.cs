using CandidateTest.Api.Models;

namespace CandidateTest.Api.Services
{
    public interface IScoreCalculator
    {
        int Calculate(Test test, List<AnswerItem> answers);
    }

    public class ScoreCalculator : IScoreCalculator
    {
        public int Calculate(Test test, List<AnswerItem> answers)
        {
            var score = 0;
            foreach (var item in answers)
            {
                var question = test.Questions.FirstOrDefault(q => q.Id == item.QuestionId);
                if (question == null) continue;

                if (question.Type == "MCQ")
                {
                    if (string.Equals(question.Answer.Trim(), item.CandidateAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        score += question.Points;
                        item.Correct = true;
                    }
                }
                else if (question.Type == "Coding")
                {
                    if (!string.IsNullOrWhiteSpace(item.CandidateAnswer))
                    {
                        score += question.Points;
                        item.Correct = true;
                    }
                }
            }
            return score;
        }
    }
}

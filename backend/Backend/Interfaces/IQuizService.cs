using System;
using System.Threading.Tasks;

public interface IQuizService {
  Task InitUser(string user);
  Task<bool> HasQuiz(string user, string quizId);

  Task<string> CreateQuiz(string user, string quizName);
  Task RemoveQuiz(string user, string quizId);
  Task AddQuizQuestion(string user, string quizId, QuizQuestion question);
  Task ChangeQuizQuestion(string user, string quizId, int questionInd, QuizQuestion question);
  Task RemoveQuizQuestion(string user, string quizId, int questionInd);
  Task<Quiz> GetQuiz(string user, string quizId);

  Task<QuizInfo[]> GetQuizzesInfo(string user);
}
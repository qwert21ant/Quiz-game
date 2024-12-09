public interface IQuizService {
  void InitUser(string user);
  bool HasQuiz(string user, string quizId);

  string CreateQuiz(string user, string quizName);
  void RemoveQuiz(string user, string quizId);
  void AddQuizQuestion(string user, string quizId, QuizQuestion question);
  void ChangeQuizQuestion(string user, string quizId, int questionInd, QuizQuestion question);
  void RemoveQuizQuestion(string user, string quizId, int questionInd);
  Quiz GetQuiz(string user, string quizId);

  QuizInfo[] GetQuizzesInfo(string user);
}
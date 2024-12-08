using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class QuizService : JsonPersistenceService<QuizzesStorage>, IQuizService {
  public QuizService(IOptions<StorageSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "quizzes.json"), new ())
    {}

  public async Task InitUser(string user) {
    if (Value.Quizzes.ContainsKey(user))
      return;
    
    await Mutate(value => {
      value.Quizzes.Add(user, new ());
    });
  }

  public async Task<bool> HasQuiz(string user, string quizId) {
    return Value.Quizzes[user].ContainsKey(quizId);
  }

  public async Task<string> CreateQuiz(string user, string quizName) {
    var id = GetNewID();
    await Mutate(value => {
      value.Quizzes[user].Add(id, new Quiz {
        Info = new QuizInfo {
          Id = id,
          Name = quizName
        }
      });
    });
    return id;
  }

  public async Task RemoveQuiz(string user, string quizId) {
    if (!Value.Quizzes[user].ContainsKey(quizId))
      throw new ServiceException("There is no quiz with id " + quizId);
    
    await Mutate(value => {
      value.Quizzes[user].Remove(quizId);
    });
  }

  public async Task AddQuizQuestion(string user, string quizId, QuizQuestion question) {
    if (!Value.Quizzes[user].ContainsKey(quizId))
      throw new ServiceException("There is no quiz with id " + quizId);

    EnsureQuizQuestionCorrect(question);

    await Mutate(value => {
      value.Quizzes[user][quizId].Questions.Add(question);
    });
  }

  public async Task ChangeQuizQuestion(string user, string quizId, int questionInd, QuizQuestion question) {
    if (!Value.Quizzes[user].ContainsKey(quizId))
      throw new ServiceException("There is no quiz with id " + quizId);
    
    if (questionInd >= Value.Quizzes[user][quizId].Questions.Count)
      throw new ServiceException("Incorrect questionInd");
    
    EnsureQuizQuestionCorrect(question);

    await Mutate(value => {
      value.Quizzes[user][quizId].Questions[questionInd] = question;
    });
  }

  public async Task RemoveQuizQuestion(string user, string quizId, int questionInd) {
    if (!Value.Quizzes[user].ContainsKey(quizId))
      throw new ServiceException("There is no quiz with id " + quizId);
    
    if (questionInd < 0 || questionInd >= Value.Quizzes[user][quizId].Questions.Count)
      throw new ServiceException("Incorrect questionInd");
    
    await Mutate(value => {
      value.Quizzes[user][quizId].Questions.RemoveAt(questionInd);
    });
  }

  public async Task<Quiz> GetQuiz(string user, string quizId) {
    if (!Value.Quizzes[user].ContainsKey(quizId))
      throw new ServiceException("There is no quiz with id " + quizId);
    
    return Value.Quizzes[user][quizId];
  }

  private void EnsureQuizQuestionCorrect(QuizQuestion question) {
    if (question.Type == QuizQuestionType.Choise) {
      if (question.Options == null)
        throw new ServiceException("question.options is null");
      
      if (question.Options.Length <= 1)
        throw new ServiceException("There must be at least two options");
      
      if (question.AnswerOptionInd == null || question.AnswerOptionInd < 0 || question.AnswerOptionInd >= question.Options.Length)
        throw new ServiceException("question.answerOptionInd is incorrect");
    } else {
      if (question.Answer == null)
        throw new ServiceException("question.answer is null");
    }
  }

  public async Task<QuizInfo[]> GetQuizzesInfo(string user) {
    return Value.Quizzes[user].Select(x => x.Value.Info).ToArray();
  }

  private string GetNewID() {
    return Guid.NewGuid().ToString();
  }
}
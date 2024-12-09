using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;

public class QuizService : JsonPersistenceService<QuizzesStorage>, IQuizService {
  public QuizService(IOptions<AppSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "quizzes.json"), new ())
    {}

  public void InitUser(string user) {
    lock (this) {
      if (Value.Quizzes.ContainsKey(user))
        return;

      Mutate(value => {
        value.Quizzes.Add(user, new ());
      });
    }
  }

  public bool HasQuiz(string user, string quizId) {
    lock (this) {
      return Value.Quizzes[user].ContainsKey(quizId);
    }
  }

  public string CreateQuiz(string user, string quizName) {
    lock (this) {
      var id = GetNewID();
      Mutate(value => {
        value.Quizzes[user].Add(id, new Quiz {
          Info = new QuizInfo {
            Id = id,
            Name = quizName
          }
        });
      });
      return id;
    }
  }

  public void RemoveQuiz(string user, string quizId) {
    lock (this) {
      if (!Value.Quizzes[user].ContainsKey(quizId))
        throw new ServiceException("There is no quiz with id " + quizId);

      Mutate(value => {
        value.Quizzes[user].Remove(quizId);
      });
    }
  }

  public void AddQuizQuestion(string user, string quizId, QuizQuestion question) {
    lock (this) {
      if (!Value.Quizzes[user].ContainsKey(quizId))
        throw new ServiceException("There is no quiz with id " + quizId);

      EnsureQuizQuestionCorrect(question);

      Mutate(value => {
        value.Quizzes[user][quizId].Questions.Add(question);
      });
    }
  }

  public void ChangeQuizQuestion(string user, string quizId, int questionInd, QuizQuestion question) {
    lock (this) {
      if (!Value.Quizzes[user].ContainsKey(quizId))
        throw new ServiceException("There is no quiz with id " + quizId);

      if (questionInd >= Value.Quizzes[user][quizId].Questions.Count)
        throw new ServiceException("Incorrect questionInd");

      EnsureQuizQuestionCorrect(question);

      Mutate(value => {
        value.Quizzes[user][quizId].Questions[questionInd] = question;
      });
    }
  }

  public void RemoveQuizQuestion(string user, string quizId, int questionInd) {
    lock (this) {
      if (!Value.Quizzes[user].ContainsKey(quizId))
        throw new ServiceException("There is no quiz with id " + quizId);

      if (questionInd < 0 || questionInd >= Value.Quizzes[user][quizId].Questions.Count)
        throw new ServiceException("Incorrect questionInd");

      Mutate(value => {
        value.Quizzes[user][quizId].Questions.RemoveAt(questionInd);
      });
    }
  }

  public Quiz GetQuiz(string user, string quizId) {
    lock (this) {
      if (!Value.Quizzes[user].ContainsKey(quizId))
        throw new ServiceException("There is no quiz with id " + quizId);

      return Value.Quizzes[user][quizId];
    }
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

  public QuizInfo[] GetQuizzesInfo(string user) {
    lock (this) {
      return Value.Quizzes[user].Select(x => x.Value.Info).ToArray();
    }
  }

  private string GetNewID() {
    return Guid.NewGuid().ToString();
  }
}

using System;
using System.Linq;
using MongoDB.Driver;

public class QuizService : IQuizService {
  private IMongoCollection<QuizDocument> collection;

  public QuizService(MongoDBService dbService) {
    collection = dbService.GetCollection<QuizDocument>("quizzes");
  }

  private FilterDefinition<QuizDocument> QuizFilter(string user, string quizId) {
    return Builders<QuizDocument>.Filter.And([
      Builders<QuizDocument>.Filter.Eq("User", user),
      Builders<QuizDocument>.Filter.Eq("Quiz.Info.Id", quizId)
    ]);
  }

  public void InitUser(string user) {
    // remove
  }

  public bool HasQuiz(string user, string quizId) {
    lock (this) {
      return collection.Find(
        QuizFilter(user, quizId)
      ).CountDocuments() == 1;
    }
  }

  public string CreateQuiz(string user, string quizName) {
    lock (this) {
      var id = GetNewID();

      collection.InsertOne(new QuizDocument {
        User = user,
        Quiz = new Quiz {
          Info = new QuizInfo {
            Id = id,
            Name = quizName
          }
        }
      });

      return id;
    }
  }

  public void RemoveQuiz(string user, string quizId) {
    lock (this) {
      collection.DeleteOne(QuizFilter(user, quizId));
    }
  }

  public void AddQuizQuestion(string user, string quizId, QuizQuestion question) {
    lock (this) {
      EnsureQuizQuestionCorrect(question);

      collection.UpdateOne(
        QuizFilter(user, quizId),
        Builders<QuizDocument>.Update.Push("Quiz.Questions", question)
      );
    }
  }

  public void ChangeQuizQuestion(string user, string quizId, int questionInd, QuizQuestion question) {
    lock (this) {
      var quiz = collection.Find(QuizFilter(user, quizId)).ToList()[0].Quiz;

      if (questionInd >= quiz.Questions.Count)
        throw new ServiceException("Incorrect questionInd");

      EnsureQuizQuestionCorrect(question);

      collection.UpdateOne(
        QuizFilter(user, quizId),
        Builders<QuizDocument>.Update.Set("Quiz.Questions." + questionInd, question)
      );
    }
  }

  public void RemoveQuizQuestion(string user, string quizId, int questionInd) {
    lock (this) {
      var quiz = collection.Find(QuizFilter(user, quizId)).ToList()[0].Quiz;

      if (questionInd < 0 || questionInd >= quiz.Questions.Count)
        throw new ServiceException("Incorrect questionInd");

      quiz.Questions.RemoveAt(questionInd);

      collection.UpdateOne(
        QuizFilter(user, quizId),
        Builders<QuizDocument>.Update.Set("Quiz.Questions", quiz.Questions)
      );
    }
  }

  public Quiz GetQuiz(string user, string quizId) {
    lock (this) {
      return collection.Find(QuizFilter(user, quizId)).ToList()[0].Quiz;
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
      var list = collection.Find(Builders<QuizDocument>.Filter.Eq("User", user)).ToList();
      return list.Select(x => x.Quiz.Info).ToArray();
    }
  }

  private string GetNewID() {
    return Guid.NewGuid().ToString();
  }
}

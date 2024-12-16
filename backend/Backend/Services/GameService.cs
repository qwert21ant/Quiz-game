using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

/*
State changes:

Waiting -[NextQuestion]> Question -[GoToResults]> Results -[EndGame]> *room closed*
                          |    ^
              [NextQuestion]  [FinishQuestion]
                          v    |
                          Answer
*/

public class GameService : IGameService {
  private IMongoCollection<GameDocument> collection;
  private IRoomService roomService;
  private IQuizService quizService;

  public GameService(MongoDBService dbService, IRoomService roomService, IQuizService quizService) {
    collection = dbService.GetCollection<GameDocument>("games");
    this.roomService = roomService;
    this.quizService = quizService;
  }

  private FilterDefinition<GameDocument> GameFilter(string user) {
    return Builders<GameDocument>.Filter.Eq("User", user);
  }

  private GameDocument GetGameDocument(string user) {
    return collection.Find(GameFilter(user)).ToList()[0];
  }

  public void InitUser(string user) {
    // remove?
  }

  public void StartGame(string user) {
    lock (this) {
      roomService.StartGame(user);

      var roomState = roomService.GetRoomState(user);

      Dictionary<string, int> leaderboard = new ();
      foreach (var participant in roomState.Participants)
        leaderboard.Add(participant, 0);

      collection.InsertOne(new GameDocument {
        User = user,
        State = new GameState {
          StateType = GameStateType.Waiting,
          Results = new GameResults {
            Leaderboard = leaderboard
          },
          NextQuestionInd = 0,
        }
      });
    }
  }

  public void EndGame(string user) {
    lock (this) {
      roomService.EndGame(user);

      collection.DeleteOne(GameFilter(user));
    }
  }

  public GameState GetAdminGameState(string user) {
    lock (this) {
      if (!roomService.GetRoomState(user).IsGameRunning)
        throw new ServiceException("You haven't started game");

      EnsureQuestionFinished(user);

      var doc = GetGameDocument(user);
      return doc.State;
    }
  }

  public void SelectNextQuestion(string user, int questionInd) {
    lock (this) {
      if (roomService.GetRoomConfig(user).NextQuestionAutoMode)
        throw new ServiceException("You are not allowed to select question in auto mode");

      var doc = GetGameDocument(user);
      if (doc.State.StateType != GameStateType.Waiting && doc.State.StateType != GameStateType.Answer)
        throw new ServiceException("Incorrect state");

      collection.UpdateOne(
        GameFilter(user),
        Builders<GameDocument>.Update
          .Set("State.NextQuestionInd", questionInd)
      );
    }
  }

  public void NextQuestion(string user) {
    lock (this) {
      var doc = GetGameDocument(user);
      if (doc.State.StateType != GameStateType.Waiting && doc.State.StateType != GameStateType.Answer)
        throw new ServiceException("Incorrect state");

      var roomConfig = roomService.GetRoomConfig(user);
      var quiz = quizService.GetQuiz(user, roomConfig.QuizId!);

      if (roomConfig.NextQuestionAutoMode && doc.State.NextQuestionInd == quiz.Questions.Count) {
        GoToResults(user);
        return;
      }

      var question = quiz.Questions[doc.State.NextQuestionInd];

      collection.UpdateOne(
        GameFilter(user),
        Builders<GameDocument>.Update
          .Set("State.StateType", GameStateType.Question)
          .Set("State.Question", new GameQuestion {
            Type = question.Type,
            Text = question.Text,
            TimeLimit = 30, // todo
            Options = question.Options,
            QuestionAppearanceTime = CurrentTs()
          })
          .Set("State.Answer", new GameAnswer {
            Answer = question.Type == QuizQuestionType.Choise ? question.Options![(int) question.AnswerOptionInd!] : question.Answer,
            AnswerOptionInd = question.AnswerOptionInd,
          })
          .Set("State.ParticipantsAnswers", new Dictionary<string, GameAnswer>())
      );
    }
  }

  public void Answer(string user, string roomId, GameAnswer answer) {
    lock (this) {
      var owner = roomService.GetRoomOwner(roomId);
      if (!roomService.GetRoomState(owner).IsGameRunning)
        throw new ServiceException("Game hasn't started yet");

      var doc = GetGameDocument(owner);
      var gameState = doc.State;
      if (gameState.Results!.Leaderboard!.ContainsKey(owner))
        throw new ServiceException("You are not in room");

      EnsureQuestionFinished(owner);

      if (gameState.StateType != GameStateType.Question)
        throw new ServiceException("You are not allowed to do this");

      if (gameState.ParticipantsAnswers!.ContainsKey(user))
        throw new ServiceException("You already answered");

      if (
        gameState.Question!.Type == QuizQuestionType.Text && answer.Answer == null
        || gameState.Question!.Type == QuizQuestionType.Choise && (
          answer.AnswerOptionInd == null || answer.AnswerOptionInd < 0
          || answer.AnswerOptionInd > gameState.Question!.Options!.Length
        )
      )
        throw new ServiceException("Incorrect answer format");

      if (gameState.Question!.Type == QuizQuestionType.Choise)
        answer.Answer = gameState.Question.Options![(int) answer.AnswerOptionInd!];

      collection.UpdateOne(
        GameFilter(owner),
        Builders<GameDocument>.Update
          .Set("State.ParticipantsAnswers." + user, answer)
          .Set("State.Results.Leaderboard." + user, gameState.Results!.Leaderboard![user] + CountScoreForAnswer(owner, answer))
      );

      if (gameState.ParticipantsAnswers!.Count + 1 == gameState.Results!.Leaderboard!.Count)
        FinishQuestion(owner);
    }
  }

  public GameState GetParticipantGameState(string user, string roomId) {
    lock (this) {
      var owner = roomService.GetRoomOwner(roomId);
      if (!roomService.GetRoomState(owner).IsGameRunning)
        throw new ServiceException("Game hasn't started yet");

      var doc = GetGameDocument(owner);
      var gameState = doc.State;
      if (gameState.Results!.Leaderboard!.ContainsKey(owner))
        throw new ServiceException("You are not in room");

      EnsureQuestionFinished(owner);

      return new GameState {
        StateType = gameState.StateType,
        Answer = gameState.StateType == GameStateType.Answer
          ? gameState.Answer : null,
        Question = gameState.StateType == GameStateType.Answer || gameState.StateType == GameStateType.Question
          ? gameState.Question : null,
        Results = gameState.StateType == GameStateType.Results
          ? new GameResults {
            Score = gameState.Results.Leaderboard[user],
            Place = gameState.Results.ParticipantsPlaces![user],
          } : null,
      };
    }
  }

  public void GoToResults(string user) {
    lock (this) {
      var doc = GetGameDocument(user);
      if (doc.State.StateType == GameStateType.Question)
        throw new ServiceException("Incorrect state");

      collection.UpdateOne(
        GameFilter(user),
        Builders<GameDocument>.Update
          .Set("State.StateType", GameStateType.Results)
          .Set("State.Results.ParticipantsPlaces",
            doc.State.Results!.Leaderboard!
              .OrderByDescending(entry => entry.Value)
              .Select((entry, index) => new {
                entry.Key,
                Value = index + 1,
              })
              .ToDictionary(x => x.Key, x => x.Value)
          )
      );
    }
  }

  public void FinishQuestion(string user) {
    lock (this) {
      var isAutoMode = roomService.GetRoomConfig(user).NextQuestionAutoMode;

      collection.UpdateOne(
        GameFilter(user),
        isAutoMode
          ? Builders<GameDocument>.Update
            .Set("State.StateType", GameStateType.Answer)
            .Inc("State.NextQuestionInd", 1)
          : Builders<GameDocument>.Update
            .Set("State.StateType", GameStateType.Answer)
      );
    }
  }

  private int CountScoreForAnswer(string user, GameAnswer answer) {
    var doc = GetGameDocument(user);
    if (
      doc.State.Question!.Type == QuizQuestionType.Text && doc.State.Answer!.Answer != answer.Answer
      || doc.State.Question!.Type == QuizQuestionType.Choise && doc.State.Answer!.AnswerOptionInd != answer.AnswerOptionInd
    )
      return 0;

    int score = 10; // for correct answer

    int tl = doc.State.Question!.TimeLimit;
    int secsFromStart = (int) (CurrentTs() - (long) doc.State.Question!.QuestionAppearanceTime!) / 1000;

    score += (int) ((tl - secsFromStart) / (double) tl * 10); // for speed

    return score;
  }

  private void EnsureQuestionFinished(string user) {
    var doc = GetGameDocument(user);
    if (doc.State.StateType != GameStateType.Question)
      return;

    if (doc.State.Question!.QuestionAppearanceTime + doc.State.Question!.TimeLimit * 1000L <= CurrentTs())
      FinishQuestion(user);
  }

  private long CurrentTs() {
    return ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds();
  }
}

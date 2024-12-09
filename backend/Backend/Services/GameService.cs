using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;

/*
State changes:

Waiting -[NextQuestion]> Question -[GoToResults]> Results -[EndGame]> *room closed*
                          |    ^
              [NextQuestion]  [FinishQuestion]
                          v    |
                          Answer
*/

public class GameService : JsonPersistenceService<GamesStorage>, IGameService {
  private IRoomService roomService;
  private IQuizService quizService;

  public GameService(IOptions<AppSettings> settings, IRoomService roomService, IQuizService quizService)
    : base(Path.Join(settings.Value.RootDir, "games.json"), new GamesStorage()) {
    this.roomService = roomService;
    this.quizService = quizService;
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

      Mutate(value => {
        value.Games.Add(user, new GameState {
          StateType = GameStateType.Waiting,
          Results = new GameResults {
            Leaderboard = leaderboard
          },
          NextQuestionInd = 0,
        });
      });
    }
  }

  public void EndGame(string user) {
    lock (this) {
      roomService.EndGame(user);

      Mutate(value => {
        value.Games.Remove(user);
      });
    }
  }

  public GameState GetAdminGameState(string user) {
    lock (this) {
      if (!Value.Games.ContainsKey(user))
        throw new ServiceException("You haven't started game");

      EnsureQuestionFinished(user);

      return Value.Games[user];
    }
  }

  public void SelectNextQuestion(string user, int questionInd) {
    lock (this) {
      if (roomService.GetRoomConfig(user).NextQuestionAutoMode)
        throw new ServiceException("You are not allowed to select question in auto mode");

      if (Value.Games[user].StateType != GameStateType.Waiting && Value.Games[user].StateType != GameStateType.Answer)
        throw new ServiceException("Incorrect state");

      Mutate(value => {
        value.Games[user].NextQuestionInd = questionInd;
      });
    }
  }

  public void NextQuestion(string user) {
    lock (this) {
      if (Value.Games[user].StateType != GameStateType.Waiting && Value.Games[user].StateType != GameStateType.Answer)
        throw new ServiceException("Incorrect state");

      var roomConfig = roomService.GetRoomConfig(user);
      var quiz = quizService.GetQuiz(user, roomConfig.QuizId!);

      if (roomConfig.NextQuestionAutoMode && Value.Games[user].NextQuestionInd == quiz.Questions.Count) {
        GoToResults(user);
        return;
      }

      var question = quiz.Questions[Value.Games[user].NextQuestionInd];

      Mutate(value => {
        value.Games[user].StateType = GameStateType.Question;
        value.Games[user].Question = new GameQuestion {
          Type = question.Type,
          Text = question.Text,
          TimeLimit = 30, // todo
          Options = question.Options,
          QuestionAppearanceTime = CurrentTs()
        };
        value.Games[user].Answer = new GameAnswer {
          Answer = question.Type == QuizQuestionType.Choise ? question.Options![(int) question.AnswerOptionInd!] : question.Answer,
          AnswerOptionInd = question.AnswerOptionInd,
        };
        value.Games[user].ParticipantsAnswers = new ();
      });
    }
  }

  public void Answer(string user, string roomId, GameAnswer answer) {
    lock (this) {
      var owner = roomService.GetRoomOwner(roomId);

      if (!Value.Games.ContainsKey(owner))
        throw new ServiceException("Game hasn't started yet");

      var gameState = Value.Games[owner];
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

      Mutate(value => {
        value.Games[owner].ParticipantsAnswers!.Add(user, answer);
        value.Games[owner].Results!.Leaderboard![user] += CountScoreForAnswer(owner, answer);
      });

      if (gameState.ParticipantsAnswers!.Count == gameState.Results!.Leaderboard!.Count)
        FinishQuestion(owner);
    }
  }

  public GameState GetParticipantGameState(string user, string roomId) {
    lock (this) {
      var owner = roomService.GetRoomOwner(roomId);

      if (!Value.Games.ContainsKey(owner))
        throw new ServiceException("Game hasn't started yet");

      var gameState = Value.Games[owner];
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
      if (Value.Games[user].StateType == GameStateType.Question)
        throw new ServiceException("Incorrect state");

      Mutate(value => {
        value.Games[user].StateType = GameStateType.Results;
        value.Games[user].Results!.ParticipantsPlaces =
          value.Games[user].Results!.Leaderboard!
          .OrderByDescending(entry => entry.Value)
          .Select((entry, index) => new {
            entry.Key,
            Value = index + 1,
          })
          .ToDictionary(x => x.Key, x => x.Value);
      });
    }
  }

  public void FinishQuestion(string user) {
    lock (this) {
      var isAutoMode = roomService.GetRoomConfig(user).NextQuestionAutoMode;

      Mutate(value => {
        value.Games[user].StateType = GameStateType.Answer;
        if (isAutoMode)
          value.Games[user].NextQuestionInd++;
      });
    }
  }

  private int CountScoreForAnswer(string user, GameAnswer answer) {
    if (
      Value.Games[user].Question!.Type == QuizQuestionType.Text && Value.Games[user].Answer!.Answer != answer.Answer
      || Value.Games[user].Question!.Type == QuizQuestionType.Choise && Value.Games[user].Answer!.AnswerOptionInd != answer.AnswerOptionInd
    )
      return 0;

    int score = 10; // for correct answer

    int tl = Value.Games[user].Question!.TimeLimit;
    int secsFromStart = (int) (CurrentTs() - (long) Value.Games[user].Question!.QuestionAppearanceTime!) / 1000;

    score += (int) ((tl - secsFromStart) / (double) tl * 10); // for speed

    return score;
  }

  private void EnsureQuestionFinished(string user) {
    if (Value.Games[user].StateType != GameStateType.Question)
      return;

    if (Value.Games[user].Question!.QuestionAppearanceTime + Value.Games[user].Question!.TimeLimit * 1000L <= CurrentTs())
      FinishQuestion(user);
  }

  private long CurrentTs() {
    return ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds();
  }
}

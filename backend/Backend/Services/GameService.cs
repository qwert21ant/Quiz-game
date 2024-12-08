using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

/*
State changes:

Waiting -> Question -> Results
            |    ^
            v    |
            Answer
*/

public class GameService : JsonPersistenceService<GamesStorage>, IGameService {
  private IRoomService roomService;
  private IQuizService quizService;
  
  public GameService(IOptions<StorageSettings> settings, IRoomService roomService, IQuizService quizService)
    : base(Path.Join(settings.Value.RootDir, "games.json"), new GamesStorage()) {
    this.roomService = roomService;
    this.quizService = quizService;
  }

  public async Task InitUser(string user) {
    // remove?
  }

  public async Task StartGame(string user) {
    await roomService.StartGame(user);

    var roomState = await roomService.GetRoomState(user);

    Dictionary<string, int> leaderboard = new ();
    foreach (var participant in roomState.Participants)
      leaderboard.Add(participant, 0);

    await Mutate(value => {
      value.Games.Add(user, new GameState {
        StateType = GameStateType.Waiting,
        Results = new GameResults {
          Leaderboard = leaderboard
        },
        NextQuestionInd = 0,
      });
    });
  }

  public async Task EndGame(string user) {
    await roomService.EndGame(user);
    
    await Mutate(value => {
      value.Games.Remove(user);
    });
  }

  public async Task<GameState> GetAdminGameState(string user) {
    if (!Value.Games.ContainsKey(user))
      throw new ServiceException("You haven't started game");
    
    await EnsureQuestionFinished(user);

    return Value.Games[user];
  }

  public async Task SelectNextQuestion(string user, int questionInd) {
    if (Value.Games[user].StateType != GameStateType.Waiting && Value.Games[user].StateType != GameStateType.Answer)
      throw new ServiceException("Incorrect state");
    
    await Mutate(value => {
      value.Games[user].NextQuestionInd = questionInd;
    });
  }

  public async Task NextQuestion(string user) {
    if (Value.Games[user].StateType != GameStateType.Waiting && Value.Games[user].StateType != GameStateType.Answer)
      throw new ServiceException("Incorrect state");
    
    var roomConfig = await roomService.GetRoomConfig(user);
    var question = (await quizService.GetQuiz(user, roomConfig.QuizId!)).Questions[Value.Games[user].NextQuestionInd];

    await Mutate(value => {
      value.Games[user].StateType = GameStateType.Question;
      value.Games[user].Question = new GameQuestion {
        Type = question.Type,
        Text = question.Text,
        TimeLimit = 30, // todo
        Options = question.Options,
        QuestionAppearanceTime = CurrentTs()
      };
      value.Games[user].Answer = new GameAnswer {
        Answer = question.Answer == null ? question.Options![(int) question.AnswerOptionInd!] : question.Answer,
        AnswerOptionInd = question.AnswerOptionInd,
      };
      value.Games[user].ParticipantsAnswers = new ();
    });
  }

  public async Task Answer(string user, string roomId, GameAnswer answer) {
    var owner = await roomService.GetRoomOwner(roomId);

    if (!Value.Games.ContainsKey(owner))
      throw new ServiceException("Game hasn't started yet");
    
    var gameState = Value.Games[owner];
    if (gameState.Results!.Leaderboard!.ContainsKey(owner))
      throw new ServiceException("You are not in room");
    
    await EnsureQuestionFinished(owner);
    
    if (gameState.StateType != GameStateType.Question)
      throw new ServiceException("You are not allowed to do this");
    
    if (gameState.ParticipantsAnswers!.ContainsKey(user))
      throw new ServiceException("You already answered");
    
    // todo: check answer

    await Mutate(value => {
      value.Games[owner].ParticipantsAnswers!.Add(user, answer);
    });

    if (Value.Games[owner].ParticipantsAnswers!.Count == Value.Games[owner].Results!.Leaderboard!.Count)
      await FinishQuestion(owner);
  }

  public async Task<GameState> GetParticipantGameState(string user, string roomId) {
    var owner = await roomService.GetRoomOwner(roomId);

    if (!Value.Games.ContainsKey(owner))
      throw new ServiceException("Game hasn't started yet");

    var gameState = Value.Games[owner];
    if (gameState.Results!.Leaderboard!.ContainsKey(owner))
      throw new ServiceException("You are not in room");

    await EnsureQuestionFinished(owner);
    
    return new GameState {
      StateType = gameState.StateType,
      Answer = gameState.StateType == GameStateType.Answer
        ? gameState.Answer : null,
      Question = gameState.StateType == GameStateType.Answer || gameState.StateType == GameStateType.Question
        ? gameState.Question : null,
      Results = gameState.StateType == GameStateType.Results
        ? gameState.Results : null,
    };
  }

  public async Task FinishQuestion(string user) {
    await Mutate(value => {
      value.Games[user].StateType = GameStateType.Answer;
      value.Games[user].NextQuestionInd++;
    });
  }

  private async Task EnsureQuestionFinished(string user) {
    if (Value.Games[user].StateType != GameStateType.Question)
      return;
    
    if (Value.Games[user].Question!.QuestionAppearanceTime + Value.Games[user].Question!.TimeLimit * 1000L <= CurrentTs())
      await FinishQuestion(user);
  }

  private long CurrentTs() {
    return ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeMilliseconds();
  }
}
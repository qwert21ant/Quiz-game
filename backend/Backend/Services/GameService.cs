using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class GameService : JsonPersistenceService<GamesStorage>, IGameService {
  private IRoomService roomService;
  private IQuizService quizService;
  
  public GameService(IOptions<StorageSettings> settings, IRoomService roomService, IQuizService quizService)
    : base(Path.Join(settings.Value.RootDir, "games.json"), new GamesStorage()) {
    this.roomService = roomService;
    this.quizService = quizService;
  }

  public async Task InitUser(string user) {
    if (Value.Games.ContainsKey(user))
      return;
    
    await Mutate(value => {
      value.Games.Add(user, new GameState {
        StateType = GameStateType.Waiting
      });
    });
  }

  public async Task InitGame(string user) {
    var roomConfig = await roomService.GetRoomConfig(user);
    var roomState = await roomService.GetRoomState(user);

    if (roomState.Participants.Count == 0)
      throw new ServiceException("Can't start game in empty room");
    
    await Mutate(value => {
      value.Games.Add(user, new GameState {
        StateType = GameStateType.Waiting,
      });
    });
  }

  public async Task StartGame(string user) {
    var roomConfig = await roomService.GetRoomConfig(user);
    var roomState = await roomService.GetRoomState(user);

    if (roomState.Participants.Count == 0)
      throw new ServiceException("Can't start game in empty room");

    Dictionary<string, int> leaderboard = new ();
    foreach (var participant in roomState.Participants)
      leaderboard.Add(participant, 0);

    await Mutate(value => {
      value.Games[user].Results = new GameResults {
        Leaderboard = leaderboard
      };
    });
  }

  public async Task<GameState> GetAdminGameState(string user) {
    if (!Value.Games.ContainsKey(user))
      throw new ServiceException("You haven't started game");
    
    return Value.Games[user];
  }

  public async Task SelectNextQuestion(string user, int questionId) {
    // todo
  }

  public async Task NextQuestion(string user) {
    // todo
  }

  public async Task Answer(string user, string roomId, GameAnswer answer) {
    var owner = await roomService.GetRoomOwner(roomId);

    if (!Value.Games.ContainsKey(owner))
      throw new ServiceException("Game hasn't started yet");
    
    var gameState = Value.Games[owner];
    if (gameState.Results!.Leaderboard!.ContainsKey(owner))
      throw new ServiceException("You are not in room");
    
    if (gameState.StateType != GameStateType.Question)
      throw new ServiceException("You are not allowed to do this");
    
    if (gameState.Answers!.ContainsKey(user))
      throw new ServiceException("You already answered");
    
    // todo: check answer

    await Mutate(value => {
      value.Games[owner].Answers!.Add(user, answer);
    });
  }

  public async Task<GameState> GetParticipantGameState(string user, string roomId) {
    var owner = await roomService.GetRoomOwner(roomId);

    if (!Value.Games.ContainsKey(owner))
      throw new ServiceException("Game hasn't started yet");

    var gameState = Value.Games[owner];
    if (gameState.Results!.Leaderboard!.ContainsKey(owner))
      throw new ServiceException("You are not in room");
    
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
}
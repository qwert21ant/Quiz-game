using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class RoomService : JsonPersistenceService<RoomsStorage>, IRoomService {
  private Random random = new Random();
  private IQuizService quizService;
  
  public RoomService(IOptions<AppSettings> settings, IQuizService quizService)
    : base(Path.Join(settings.Value.RootDir, "rooms.json"), new RoomsStorage()) {
    this.quizService = quizService;
  }

  public async Task InitUser(string user) {
    if (Value.Rooms.ContainsKey(user))
      return;
    
    await Mutate(value => {
      value.Rooms.Add(user, new Room {
        Config = new RoomConfig {
          Info = new RoomInfo {
            Name = user + "\'s room"
          },
          MaxParticipants = 10
        },
        State = new ()
      });
    });
  }

  public async Task StartGame(string user) {
    if (Value.Rooms[user].State.IsGameRunning)
      throw new ServiceException("Game is already running");

    if (Value.Rooms[user].State.Participants.Count == 0)
      throw new ServiceException("Can't start game in empty room");

    await Mutate(value => {
      value.Rooms[user].State.IsGameRunning = true;
    });
  }

  public async Task EndGame(string user) {
    if (!Value.Rooms[user].State.IsGameRunning)
      throw new ServiceException("Game is not running");

    await CloseRoom(user);
  }

  public async Task<RoomConfig> GetRoomConfig(string user) {
    return Value.Rooms[user].Config;
  }

  public async Task UpdateConfig(string user, RoomConfig roomConfig) {
    if (roomConfig.QuizId == null)
      throw new ServiceException("quizId is null");
    
    if (!await quizService.HasQuiz(user, roomConfig.QuizId))
      throw new ServiceException("There is no quiz with id " + roomConfig.QuizId);

    await Mutate(value => {
      value.Rooms[user].Config = roomConfig;
    });
  }

  public async Task<RoomState> GetRoomState(string user) {
    return Value.Rooms[user].State;
  }

  public async Task OpenRoom(string user) {
    if (Value.Rooms[user].State.IsOpen)
      throw new ServiceException("Room already opened");
    
    if (!await quizService.HasQuiz(user, Value.Rooms[user].Config.QuizId!))
      throw new ServiceException("There is no quiz with id " + Value.Rooms[user].Config.QuizId);

    await Mutate(value => {
      var id = GetUniqueRoomID();
      value.Rooms[user].State.IsOpen = true;
      value.Rooms[user].State.Id = id;
      value.RoomIdToUser.Add(id, user);
    });
  }

  public async Task CloseRoom(string user) {
    await Mutate(value => {
      var id = value.Rooms[user].State.Id!;
      value.Rooms[user].State = new ();
      value.RoomIdToUser.Remove(id);
    });
  }

  public async Task KickParticipant(string user, string participant) {
    if (user == participant)
      throw new ServiceException("You cant kick yourself");

    if (!Value.Rooms[user].State.Participants.Contains(participant))
      throw new ServiceException("There is no participant " + participant + " in your this room");

    await Mutate(value => {
      value.Rooms[user].State.Participants.Remove(participant);
    });
  }

  public async Task JoinRoom(string user, string roomId) {
    var owner = await GetRoomOwner(roomId);
    if (user == owner)
      throw new ServiceException("Owner of room cant join room");

    if (Value.Rooms[owner].State.Participants.Contains(user))
      throw new ServiceException("You are already joined room");
    
    if (Value.Rooms[owner].Config.MaxParticipants == Value.Rooms[owner].State.Participants.Count)
      throw new ServiceException("Room is full");

    await Mutate(value => {
      value.Rooms[owner].State.Participants.Add(user);
    });
  }

  public async Task LeaveRoom(string user, string roomId) {
    var owner = await GetRoomOwner(roomId);
    if (user == owner)
      throw new ServiceException("Owner of room cant join room");

    await Mutate(value => {
      value.Rooms[owner].State.Participants.Remove(user);
    });
  }

  public async Task<RoomInfo> GetRoomInfo(string user, string roomId) {
    var owner = await GetRoomOwner(roomId);

    if (!Value.Rooms[owner].State.Participants.Contains(user))
      throw new ServiceException("You are not in room");
    
    return (await GetRoomConfig(owner)).Info;
  }

  public async Task<bool> GetIsGameRunning(string user, string roomId) {
    var owner = await GetRoomOwner(roomId);
    return Value.Rooms[owner].State.IsGameRunning;
  }

  public async Task<string> GetRoomOwner(string roomId) {
    if (!Value.RoomIdToUser.ContainsKey(roomId))
      throw new ServiceException("Room with id " + roomId + " not found");

    return Value.RoomIdToUser[roomId];
  }

  private string GetUniqueRoomID() {
    while (true) {
      string id = GetNewRoomID();
      if (!Value.RoomIdToUser.ContainsKey(id))
        return id;
    }
  }

  private string GetNewRoomID() {
    return random.Next(0, 100_000_000).ToString();
  }
}
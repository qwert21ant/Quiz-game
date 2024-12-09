using System;
using System.IO;
using Microsoft.Extensions.Options;

public class RoomService : JsonPersistenceService<RoomsStorage>, IRoomService {
  private Random random = new Random();
  private IQuizService quizService;

  public RoomService(IOptions<AppSettings> settings, IQuizService quizService)
    : base(Path.Join(settings.Value.RootDir, "rooms.json"), new RoomsStorage()) {
    this.quizService = quizService;
  }

  public void InitUser(string user) {
    lock (this) {
      if (Value.Rooms.ContainsKey(user))
        return;

      Mutate(value => {
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
  }

  public void StartGame(string user) {
    lock (this) {
      if (Value.Rooms[user].State.IsGameRunning)
        throw new ServiceException("Game is already running");

      if (Value.Rooms[user].State.Participants.Count == 0)
        throw new ServiceException("Can't start game in empty room");

      Mutate(value => {
        value.Rooms[user].State.IsGameRunning = true;
      });
    }
  }

  public void EndGame(string user) {
    lock (this) {
      if (!Value.Rooms[user].State.IsGameRunning)
        throw new ServiceException("Game is not running");

      CloseRoom(user);
    }
  }

  public RoomConfig GetRoomConfig(string user) {
    lock (this) {
      return Value.Rooms[user].Config;
    }
  }

  public void UpdateConfig(string user, RoomConfig roomConfig) {
    lock (this) {
      if (roomConfig.QuizId == null)
        throw new ServiceException("quizId is null");

      if (!quizService.HasQuiz(user, roomConfig.QuizId))
        throw new ServiceException("There is no quiz with id " + roomConfig.QuizId);

      Mutate(value => {
        value.Rooms[user].Config = roomConfig;
      });
    }
  }

  public RoomState GetRoomState(string user) {
    lock (this) {
      return Value.Rooms[user].State;
    }
  }

  public void OpenRoom(string user) {
    lock (this) {
      if (Value.Rooms[user].State.IsOpen)
        throw new ServiceException("Room already opened");

      if (!quizService.HasQuiz(user, Value.Rooms[user].Config.QuizId!))
        throw new ServiceException("There is no quiz with id " + Value.Rooms[user].Config.QuizId);

      Mutate(value => {
        var id = GetUniqueRoomID();
        value.Rooms[user].State.IsOpen = true;
        value.Rooms[user].State.Id = id;
        value.RoomIdToUser.Add(id, user);
      });
    }
  }

  public void CloseRoom(string user) {
    lock (this) {
      Mutate(value => {
        var id = value.Rooms[user].State.Id!;
        value.Rooms[user].State = new ();
        value.RoomIdToUser.Remove(id);
      });
    }
  }

  public void KickParticipant(string user, string participant) {
    lock (this) {
      if (user == participant)
        throw new ServiceException("You cant kick yourself");

      if (!Value.Rooms[user].State.Participants.Contains(participant))
        throw new ServiceException("There is no participant " + participant + " in your this room");

      Mutate(value => {
        value.Rooms[user].State.Participants.Remove(participant);
      });
    }
  }

  public void JoinRoom(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      if (user == owner)
        throw new ServiceException("Owner of room cant join room");

      if (Value.Rooms[owner].State.Participants.Contains(user))
        throw new ServiceException("You are already joined room");

      if (Value.Rooms[owner].Config.MaxParticipants == Value.Rooms[owner].State.Participants.Count)
        throw new ServiceException("Room is full");

      Mutate(value => {
        value.Rooms[owner].State.Participants.Add(user);
      });
    }
  }

  public void LeaveRoom(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      if (user == owner)
        throw new ServiceException("Owner of room cant join room");

      Mutate(value => {
        value.Rooms[owner].State.Participants.Remove(user);
      });
    }
  }

  public RoomInfo GetRoomInfo(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);

      if (!Value.Rooms[owner].State.Participants.Contains(user))
        throw new ServiceException("You are not in room");

      return GetRoomConfig(owner).Info;
    }
  }

  public bool GetIsGameRunning(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      return Value.Rooms[owner].State.IsGameRunning;
    }
  }

  public string GetRoomOwner(string roomId) {
    lock (this) {
      if (!Value.RoomIdToUser.ContainsKey(roomId))
        throw new ServiceException("Room with id " + roomId + " not found");

      return Value.RoomIdToUser[roomId];
    }
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

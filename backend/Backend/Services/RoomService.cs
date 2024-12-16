using System;
using MongoDB.Driver;

public class RoomService : IRoomService {
  private IMongoCollection<RoomDocument> collection;
  private IQuizService quizService;
  private Random random = new Random();

  public RoomService(MongoDBService dbService, IQuizService quizService) {
    collection = dbService.GetCollection<RoomDocument>("rooms");
    this.quizService = quizService;
  }

  private FilterDefinition<RoomDocument> RoomFilter(string user) {
    return Builders<RoomDocument>.Filter.Eq("User", user);
  }

  private RoomDocument GetRoomDocument(string user) {
    return collection.Find(RoomFilter(user)).ToList()[0];
  }

  public void InitUser(string user) {
    lock (this) {
      if (collection.Find(RoomFilter(user)).CountDocuments() == 1)
        return;

      collection.InsertOne(new RoomDocument {
        User = user,
        Room = new Room {
          Config = new RoomConfig {
            Info = new RoomInfo {
              Name = user + "\'s room"
            },
            MaxParticipants = 10
          },
          State = new ()
        }
      });
    }
  }

  public void StartGame(string user) {
    lock (this) {
      var doc = GetRoomDocument(user);
      if (doc.Room.State.IsGameRunning)
        throw new ServiceException("Game is already running");

      if (doc.Room.State.Participants.Count == 0)
        throw new ServiceException("Can't start game in empty room");

      collection.UpdateOne(
        RoomFilter(user),
        Builders<RoomDocument>.Update.Set("Room.State.IsGameRunning", true)
      );
    }
  }

  public void EndGame(string user) {
    lock (this) {
      var doc = GetRoomDocument(user);
      if (!doc.Room.State.IsGameRunning)
        throw new ServiceException("Game is not running");

      CloseRoom(user);
    }
  }

  public RoomConfig GetRoomConfig(string user) {
    lock (this) {
      return GetRoomDocument(user).Room.Config;
    }
  }

  public RoomState GetRoomState(string user) {
    lock (this) {
      return GetRoomDocument(user).Room.State;
    }
  }

  public void UpdateConfig(string user, RoomConfig roomConfig) {
    lock (this) {
      if (roomConfig.QuizId == null)
        throw new ServiceException("quizId is null");

      if (!quizService.HasQuiz(user, roomConfig.QuizId))
        throw new ServiceException("There is no quiz with id " + roomConfig.QuizId);

      collection.UpdateOne(
        RoomFilter(user),
        Builders<RoomDocument>.Update.Set("Room.Config", roomConfig)
      );
    }
  }

  public void OpenRoom(string user) {
    lock (this) {
      var doc = GetRoomDocument(user);
      if (doc.Room.State.IsOpen)
        throw new ServiceException("Room already opened");

      if (!quizService.HasQuiz(user, doc.Room.Config.QuizId!))
        throw new ServiceException("There is no quiz with id " + doc.Room.Config.QuizId);

      var id = GetUniqueRoomID();
      collection.UpdateOne(
        RoomFilter(user),
        Builders<RoomDocument>.Update
          .Set("Room.State.IsOpen", true)
          .Set("Room.State.Id", id)
      );
    }
  }

  public void CloseRoom(string user) {
    lock (this) {
      collection.UpdateOne(
        RoomFilter(user),
        Builders<RoomDocument>.Update
          .Set("Room.State", new RoomState())
      );
    }
  }

  public void KickParticipant(string user, string participant) {
    lock (this) {
      if (user == participant)
        throw new ServiceException("You cant kick yourself");

      var doc = GetRoomDocument(user);
      if (!doc.Room.State.Participants.Contains(participant))
        throw new ServiceException("There is no participant " + participant + " in your this room");

      doc.Room.State.Participants.Remove(participant);
      collection.UpdateOne(
        RoomFilter(user),
        Builders<RoomDocument>.Update
          .Set("Room.State.Participants", doc.Room.State.Participants)
      );
    }
  }

  public void JoinRoom(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      if (user == owner)
        throw new ServiceException("Owner of room cant join room");

      var doc = GetRoomDocument(owner);
      if (doc.Room.State.Participants.Contains(user))
        throw new ServiceException("You are already joined room");

      if (doc.Room.Config.MaxParticipants == doc.Room.State.Participants.Count)
        throw new ServiceException("Room is full");

      collection.UpdateOne(
        RoomFilter(owner),
        Builders<RoomDocument>.Update
          .Push("Room.State.Participants", user)
      );
    }
  }

  public void LeaveRoom(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      if (user == owner)
        throw new ServiceException("Owner of room cant join room");

      var doc = GetRoomDocument(user);

      doc.Room.State.Participants.Remove(user);
      collection.UpdateOne(
        RoomFilter(owner),
        Builders<RoomDocument>.Update
          .Set("Room.State.Participants", doc.Room.State.Participants)
      );
    }
  }

  public RoomInfo GetRoomInfo(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      var doc = GetRoomDocument(owner);

      if (!doc.Room.State.Participants.Contains(user))
        throw new ServiceException("You are not in room");

      return GetRoomConfig(owner).Info;
    }
  }

  public bool GetIsGameRunning(string user, string roomId) {
    lock (this) {
      var owner = GetRoomOwner(roomId);
      return GetRoomState(owner).IsGameRunning;
    }
  }

  public string GetRoomOwner(string roomId) {
    lock (this) {
      var list = collection.Find(
        Builders<RoomDocument>.Filter.Eq("Room.State.Id", roomId)
      ).ToList();
      if (list.Count == 0)
        throw new ServiceException("Room with id " + roomId + " not found");

      return list[0].User;
    }
  }

  private string GetUniqueRoomID() {
    while (true) {
      string id = GetNewRoomID();
      var list = collection.Find(
        Builders<RoomDocument>.Filter.Eq("Room.State.Id", id)
      ).ToList();
      if (list.Count == 0)
        return id;
    }
  }

  private string GetNewRoomID() {
    return random.Next(0, 100_000_000).ToString();
  }
}

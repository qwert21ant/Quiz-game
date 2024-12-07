using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class RoomService : JsonPersistenceService<RoomsStorage>, IRoomService {
  private Random random = new Random();
  
  public RoomService(IOptions<StorageSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "rooms.json"), new RoomsStorage())
    {}

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
        State = new RoomState {
          Open = false
        }
      });
    });
  }

  public async Task<RoomConfig> GetRoomConfig(string user) {
    return Value.Rooms[user].Config;
  }

  public async Task UpdateConfig(string user, RoomConfig roomConfig) {
    await Mutate(value => {
      value.Rooms[user].Config = roomConfig;
    });
  }

  public async Task<RoomState> GetRoomState(string user) {
    return Value.Rooms[user].State;
  }

  public async Task OpenRoom(string user) {
    if (Value.Rooms[user].State.Open)
      throw new ServiceException("Room already opened");

    await Mutate(value => {
      var id = GetUniqueRoomID();
      value.Rooms[user].State.Open = true;
      value.Rooms[user].State.Id = id;
      value.RoomIdToUser.Add(id, user);
    });
  }

  public async Task CloseRoom(string user) {
    await Mutate(value => {
      var id = value.Rooms[user].State.Id!;
      value.Rooms[user].State = new RoomState {
        Open = false,
      };
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
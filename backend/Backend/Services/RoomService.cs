using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class RoomService : JsonPersistenceService<Dictionary<string, Room>>, IRoomService {
  private IPasswordHasher<string> hasher = new PasswordHasher<string>();

  public RoomService(IOptions<StorageSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "rooms.json"), new Dictionary<string, Room>())
    {}

  private async Task<Room> EnsureRoom(string user) {
    if (Value.TryGetValue(user, out var room))
      return room;
    
    await Mutate(value => {
      value.Add(user, new Room {
        Config = new RoomConfig {
          Name = user + "\'s room",
          MaxParticipants = 10
        }
      });
    });

    return Value[user];
  }

  public async Task<RoomConfig> GetRoomConfig(string user) {
    return (await EnsureRoom(user)).Config;
  }

  public async Task UpdateConfig(string user, RoomConfig roomConfig) {
    await EnsureRoom(user);
    
    await Mutate(value => {
      value[user].Config = roomConfig;
    });
  }
}
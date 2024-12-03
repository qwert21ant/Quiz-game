using System.Threading.Tasks;

public interface IRoomService {
  Task<RoomConfig> GetRoomConfig(string user);
  Task UpdateConfig(string user, RoomConfig roomConfig);
}
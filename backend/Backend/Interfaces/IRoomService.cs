using System.Threading.Tasks;

public interface IRoomService {
  Task<RoomConfig> GetRoomConfig(string user);
  Task UpdateConfig(string user, RoomConfig roomConfig);

  Task<RoomState> GetRoomState(string user);
  Task OpenRoom(string user);
  Task CloseRoom(string user);
  Task KickParticipant(string user, string participant);

  Task JoinRoom(string user, string roomId);
  Task LeaveRoom(string user, string roomId);
}
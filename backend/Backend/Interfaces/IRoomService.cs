using System.Threading.Tasks;

public interface IRoomService {
  Task InitUser(string user);
  
  // admin
  Task<RoomConfig> GetRoomConfig(string user);
  Task UpdateConfig(string user, RoomConfig roomConfig);

  Task<RoomState> GetRoomState(string user);
  Task OpenRoom(string user);
  Task CloseRoom(string user);
  Task KickParticipant(string user, string participant);

  // participant
  Task JoinRoom(string user, string roomId);
  Task LeaveRoom(string user, string roomId);
  Task<RoomInfo> GetRoomInfo(string user, string roomId);

  // other
  Task<string> GetRoomOwner(string roomId);
}
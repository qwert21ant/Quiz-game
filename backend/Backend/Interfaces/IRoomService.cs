public interface IRoomService {
  void InitUser(string user);
  void StartGame(string user);
  void EndGame(string user);
  
  // admin
  RoomConfig GetRoomConfig(string user);
  void UpdateConfig(string user, RoomConfig roomConfig);

  RoomState GetRoomState(string user);
  void OpenRoom(string user);
  void CloseRoom(string user);
  void KickParticipant(string user, string participant);

  // participant
  void JoinRoom(string user, string roomId);
  void LeaveRoom(string user, string roomId);
  RoomInfo GetRoomInfo(string user, string roomId);
  bool GetIsGameRunning(string user, string roomId);

  // other
  string GetRoomOwner(string roomId);
}
public interface IGameService {
  void InitUser(string user);
  
  // admin
  void StartGame(string user);
  void EndGame(string user);
  GameState GetAdminGameState(string user);
  void SelectNextQuestion(string user, int questionInd);
  void NextQuestion(string user);
  void GoToResults(string user);

  // participant
  void Answer(string user, string roomId, GameAnswer answer);
  GameState GetParticipantGameState(string user, string roomId);
}
using System.Threading.Tasks;

public interface IGameService {
  Task InitUser(string user);
  
  // admin
  Task StartGame(string user);
  Task EndGame(string user);
  Task<GameState> GetAdminGameState(string user);
  Task SelectNextQuestion(string user, int questionInd);
  Task NextQuestion(string user);

  // participant
  Task Answer(string user, string roomId, GameAnswer answer);
  Task<GameState> GetParticipantGameState(string user, string roomId);
}
public class UserService : IUserService {
  private IQuizService quizService;
  private IRoomService roomService;
  private IGameService gameService;

  public UserService(IQuizService quizService, IRoomService roomService, IGameService gameService) {
    this.quizService = quizService;
    this.roomService = roomService;
    this.gameService = gameService;
  }

  public void InitUser(string user) {
    lock (this) {
      quizService.InitUser(user);
      roomService.InitUser(user);
      gameService.InitUser(user);
    }
  }

  public UserData GetUserData(string user) {
    lock (this) {
      return new UserData {
        Username = user,
        Quizzes = quizService.GetQuizzesInfo(user)
      };
    }
  }
}

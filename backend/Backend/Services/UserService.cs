using System.Threading.Tasks;

public class UserService : IUserService {
  private IQuizService quizService;
  private IRoomService roomService;
  private IGameService gameService;
  
  public UserService(IQuizService quizService, IRoomService roomService, IGameService gameService) {
    this.quizService = quizService;
    this.roomService = roomService;
    this.gameService = gameService;
  }

  public async Task InitUser(string user) {
    await quizService.InitUser(user);
    await roomService.InitUser(user);
    await gameService.InitUser(user);
  }

  public async Task<UserData> GetUserData(string user) {
    return new UserData {
      Username = user,
      Quizzes = await quizService.GetQuizzesInfo(user)
    };
  }
}
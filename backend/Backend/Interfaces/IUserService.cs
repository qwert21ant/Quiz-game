using System.Threading.Tasks;

public interface IUserService {
  Task<UserData> GetUserData(string username);
}
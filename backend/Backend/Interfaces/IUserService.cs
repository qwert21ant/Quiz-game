using System.Threading.Tasks;

public interface IUserService {
  Task InitUser(string user);
  Task<UserData> GetUserData(string user);
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class UserService : JsonPersistenceService<Dictionary<string, UserData>>, IUserService {
  private IPasswordHasher<string> hasher = new PasswordHasher<string>();

  public UserService(IOptions<StorageSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "users.json"), new Dictionary<string, UserData>())
    {}

  public async Task<UserData> GetUserData(string username) {
    if (!Value.TryGetValue(username, out var userData)) {
      userData = new UserData {
        Username = username,
      };

      await Mutate(value => {
        value.Add(username, userData);
      });
    }

    return userData;
  }
}
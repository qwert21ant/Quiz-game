using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

public class UserService : JsonPersistenceService<Dictionary<string, UserData>>, IUserService {
  public UserService(IOptions<StorageSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "users.json"), new Dictionary<string, UserData>())
    {}

  public async Task<UserData> GetUserData(string user) {
    if (Value.TryGetValue(user, out var userData))
      return userData;

    await Mutate(value => {
      value.Add(user, new UserData {
        Username = user,
      });
    });

    return Value[user];
  }
}
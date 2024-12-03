using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class CredentialsService : JsonPersistenceService<Dictionary<string, string>>, ICredentialsService {
  private IPasswordHasher<string> hasher = new PasswordHasher<string>();

  public CredentialsService(IOptions<StorageSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "credentials.json"), new Dictionary<string, string>())
    {}

  public bool Exists(string username) {
    return Value.ContainsKey(username);
  }

  public async Task Add(Credentials creds) {
    await Mutate(value => {
      value.Add(creds.Username, hasher.HashPassword(creds.Username, creds.Password));
    });
  }

  public bool Validate(Credentials creds) {
    string? hash;
    if (!Value.TryGetValue(creds.Username, out hash))
      return false;

    return hasher.VerifyHashedPassword(creds.Username, hash, creds.Password) != 0;
  }
}
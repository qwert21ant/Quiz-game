using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

public class CredentialsService : JsonPersistenceService<Dictionary<string, string>>, ICredentialsService {
  private IPasswordHasher<string> hasher = new PasswordHasher<string>();

  public CredentialsService(IOptions<AppSettings> settings)
    : base(Path.Join(settings.Value.RootDir, "credentials.json"), new Dictionary<string, string>())
    {}

  public bool Exists(string username) {
    lock(this) {
      return Value.ContainsKey(username);
    }
  }

  public void Add(Credentials creds) {
    lock(this) {
      Mutate(value => {
        value.Add(creds.Username, hasher.HashPassword(creds.Username, creds.Password));
      });
    }
  }

  public bool Validate(Credentials creds) {
    lock(this) {
      string? hash;
      if (!Value.TryGetValue(creds.Username, out hash))
        return false;

      return hasher.VerifyHashedPassword(creds.Username, hash, creds.Password) != 0;
    }
  }
}
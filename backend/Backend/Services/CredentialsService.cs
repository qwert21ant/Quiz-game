using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

public class CredentialsService : ICredentialsService {
  private IPasswordHasher<string> hasher = new PasswordHasher<string>();
  private Dictionary<string, string> data = new Dictionary<string, string>();

  public bool Exists(string username) {
    return data.ContainsKey(username);
  }

  public void Add(Credentials creds) {
    data.Add(creds.Username, hasher.HashPassword(creds.Username, creds.Password));
  }

  public bool Validate(Credentials creds) {
    string? hash;
    if (!data.TryGetValue(creds.Username, out hash))
      return false;

    return hasher.VerifyHashedPassword(creds.Username, hash, creds.Password) != 0;
  }
}
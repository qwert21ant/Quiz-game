using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;

public class CredentialsService : ICredentialsService {
  private IMongoCollection<CredentialsDocument> collection;
  private IPasswordHasher<string> hasher = new PasswordHasher<string>();

  public CredentialsService(MongoDBService dbService) {
    collection = dbService.GetCollection<CredentialsDocument>("users");
  }

  public bool Exists(string username) {
    lock(this) {
      return collection
        .Find(new BsonDocument("Value.Username", username))
        .CountDocuments() == 1;
    }
  }

  public void Add(Credentials creds) {
    lock(this) {
      collection.InsertOne(new CredentialsDocument {
        Value = new Credentials {
          Username = creds.Username,
          Password = hasher.HashPassword(creds.Username, creds.Password),
        }
      });
    }
  }

  public bool Validate(Credentials creds) {
    lock(this) {
      var list = collection
        .Find(new BsonDocument("Value.Username", creds.Username))
        .ToList();

      if (list.Count == 0)
        return false;

      string hash = list[0].Value.Password;
      return hasher.VerifyHashedPassword(creds.Username, hash, creds.Password) != 0;
    }
  }
}
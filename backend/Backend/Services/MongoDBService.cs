using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class MongoDBService {
  private MongoClient client;
  private IMongoDatabase db;

  public MongoDBService(IOptions<AppSettings> settings) {
    client = new MongoClient(settings.Value.DBConnection);
    db = client.GetDatabase(settings.Value.DBName);
  }

  public IMongoCollection<T> GetCollection<T>(string name) {
    return db.GetCollection<T>(name);
  }
}
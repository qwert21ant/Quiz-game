using MongoDB.Bson;

public class CredentialsDocument {
  public ObjectId Id { get; set; } = new ();
  public required Credentials Value { get; set; }
}
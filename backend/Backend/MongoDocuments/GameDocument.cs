using MongoDB.Bson;

public class GameDocument {
  public ObjectId Id { get; set; } = new ();
  public required string User { get; set; }
  public required GameState State { get; set; }
}
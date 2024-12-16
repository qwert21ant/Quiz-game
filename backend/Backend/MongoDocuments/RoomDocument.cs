using MongoDB.Bson;

public class RoomDocument {
  public ObjectId Id { get; set; } = new ();
  public required string User { get; set; }
  public required Room Room { get; set; }
}
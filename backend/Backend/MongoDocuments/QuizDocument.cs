using MongoDB.Bson;

public class QuizDocument {
  public ObjectId Id { get; set; } = new ();
  public required string User { get; set; }
  public required Quiz Quiz { get; set; }
}
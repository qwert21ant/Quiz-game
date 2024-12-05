using System.Text.Json.Serialization;

public class RoomConfig {
  [JsonPropertyName("info")]
  public required RoomPublicInfo Info { get; set; }

  [JsonPropertyName("quizName")]
  public string? QuizName { get; set; }

  [JsonPropertyName("maxParticipants")]
  public required int MaxParticipants { get; set; }
}
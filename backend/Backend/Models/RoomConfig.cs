using System.Text.Json.Serialization;

public class RoomConfig {
  [JsonPropertyName("name")]
  public required string Name { get; set; }

  [JsonPropertyName("description")]
  public string? Description { get; set; }

  [JsonPropertyName("quizName")]
  public string? QuizName { get; set; }

  [JsonPropertyName("maxParticipants")]
  public required int MaxParticipants { get; set; }
}
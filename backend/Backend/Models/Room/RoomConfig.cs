using System.Text.Json.Serialization;

public class RoomConfig {
  [JsonPropertyName("info")]
  public required RoomInfo Info { get; set; }

  [JsonPropertyName("quizId")]
  public string? QuizId { get; set; }

  [JsonPropertyName("maxParticipants")]
  public required int MaxParticipants { get; set; }

  [JsonPropertyName("nextQuestionAutoMode")]
  public bool NextQuestionAutoMode { get; set; } = true;
}
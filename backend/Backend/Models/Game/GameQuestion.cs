using System.Text.Json.Serialization;

public class GameQuestion {
  [JsonPropertyName("type")]
  public required QuizQuestionType Type { get; set; }

  [JsonPropertyName("text")]
  public required string Text { get; set; }

  [JsonPropertyName("timeLimit")]
  public required int TimeLimit { get; set; }

  [JsonPropertyName("options")]
  public string[]? Options { get; set; }

  [JsonPropertyName("questionAppearanceTime")]
  public long? QuestionAppearanceTime { get; set; }
}
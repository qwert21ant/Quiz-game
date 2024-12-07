using System.Text.Json.Serialization;

public class GameQuestion {
  [JsonPropertyName("text")]
  public required string Text { get; set; }

  [JsonPropertyName("options")]
  public object[]? Options { get; set; }
}
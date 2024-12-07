using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GameState {
  [JsonPropertyName("type")]
  public required GameStateType StateType { get; set; }

  [JsonPropertyName("question")]
  public GameQuestion? Question { get; set; }

  [JsonPropertyName("answer")]
  public GameAnswer? Answer { get; set; }

  [JsonPropertyName("results")]
  public GameResults? Results { get; set; }

  [JsonPropertyName("allAnswers")]
  public Dictionary<string, GameAnswer>? Answers { get; set; }
}
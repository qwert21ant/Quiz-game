using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GameResults {
  [JsonPropertyName("score")]
  public int? Score { get; set; }

  [JsonPropertyName("leaderboard")]
  public Dictionary<string, int>? Leaderboard { get; set; }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GameResults {
  [JsonPropertyName("score")]
  public int? Score { get; set; }

  [JsonPropertyName("place")]
  public int? Place { get; set; }

  [JsonPropertyName("leaderboard")]
  public Dictionary<string, int>? Leaderboard { get; set; }

  [JsonPropertyName("participantsPlaces")]
  public Dictionary<string, int>? ParticipantsPlaces { get; set; }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class GamesStorage {
  [JsonPropertyName("games")]
  public Dictionary<string, GameState> Games { get; set; } = new ();
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RoomState {
  [JsonPropertyName("isOpen")]
  public bool IsOpen { get; set; } = false;

  [JsonPropertyName("isGameRunning")]
  public bool IsGameRunning { get; set; } = false;

  [JsonPropertyName("id")]
  public string? Id { get; set; }

  [JsonPropertyName("participants")]
  public HashSet<string> Participants { get; set; } = new ();
}
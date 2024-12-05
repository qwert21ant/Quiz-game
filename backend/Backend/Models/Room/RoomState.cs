using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RoomState {
  [JsonPropertyName("open")]
  public bool Open { get; set; }

  [JsonPropertyName("id")]
  public string? Id { get; set; }

  [JsonPropertyName("participants")]
  public HashSet<string> Participants { get; set; } = new ();
}
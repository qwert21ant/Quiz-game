using System.Text.Json.Serialization;

public class Room {
  [JsonPropertyName("config")]
  public required RoomConfig Config { get; set; }

  [JsonPropertyName("state")]
  public required RoomState State { get; set; }
}
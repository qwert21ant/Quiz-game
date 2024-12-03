using System.Text.Json.Serialization;

public class Room {
  [JsonPropertyName("config")]
  public required RoomConfig Config { get; set; }
}
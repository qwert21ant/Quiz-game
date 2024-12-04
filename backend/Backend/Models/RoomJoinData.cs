using System.Text.Json.Serialization;

public class RoomJoinData {
  [JsonPropertyName("id")]
  public required string Id { get; set; }
}
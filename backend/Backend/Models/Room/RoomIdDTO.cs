using System.Text.Json.Serialization;

public class RoomIdDTO {
  [JsonPropertyName("id")]
  public required string Id { get; set; }
}
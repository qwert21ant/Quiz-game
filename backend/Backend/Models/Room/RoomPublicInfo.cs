using System.Text.Json.Serialization;

public class RoomPublicInfo {
  [JsonPropertyName("name")]
  public required string Name { get; set; }

  [JsonPropertyName("description")]
  public string? Description { get; set; }
}
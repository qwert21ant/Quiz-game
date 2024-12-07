using System.Text.Json.Serialization;

public class StringDTO {
  [JsonPropertyName("value")]
  public required string Value { get; set; }
}
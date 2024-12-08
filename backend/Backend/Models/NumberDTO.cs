using System.Text.Json.Serialization;

public class NumberDTO {
  [JsonPropertyName("value")]
  public required int Value { get; set; }
}
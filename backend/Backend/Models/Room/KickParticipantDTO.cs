using System.Text.Json.Serialization;

public class KickParticipantDTO {
  [JsonPropertyName("participant")]
  public required string Participant { get; set; }
}
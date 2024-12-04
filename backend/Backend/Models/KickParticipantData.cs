using System.Text.Json.Serialization;

public class KickParticipantData {
  [JsonPropertyName("participant")]
  public required string Participant { get; set; }
}
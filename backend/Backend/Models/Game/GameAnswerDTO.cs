using System.Text.Json.Serialization;

public class GameAnswerDTO {
  [JsonPropertyName("roomId")]
  public required string RoomId { get; set; }
  
  [JsonPropertyName("answer")]
  public required GameAnswer Answer { get; set; }
}
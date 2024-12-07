using System.Text.Json.Serialization;

public class GameAnswer {
  [JsonPropertyName("answer")]
  public string? Answer { get; set; }
  
  [JsonPropertyName("answerOptionInd")]
  public int? AnswerOptionInd { get; set; }
}
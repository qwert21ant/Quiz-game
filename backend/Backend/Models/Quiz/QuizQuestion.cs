using System.Text.Json.Serialization;

public class QuizQuestion {
  [JsonPropertyName("type")]
  public required QuizQuestionType Type { get; set; }

  [JsonPropertyName("text")]
  public required string Text { get; set; }

  [JsonPropertyName("answer")]
  public object? Answer { get; set; }

  [JsonPropertyName("options")]
  public object[]? Options { get; set; }

  [JsonPropertyName("answerOptionInd")]
  public int? AnswerOptionInd { get; set; }
}
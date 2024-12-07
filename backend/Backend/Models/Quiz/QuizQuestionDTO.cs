using System.Text.Json.Serialization;

public class QuizQuestionDTO {
  [JsonPropertyName("quizId")]
  public required string QuizId { get; set; }

  [JsonPropertyName("questionInd")]
  public int? QuestionInd { get; set; }

  [JsonPropertyName("question")]
  public QuizQuestion? Question { get; set; }
}
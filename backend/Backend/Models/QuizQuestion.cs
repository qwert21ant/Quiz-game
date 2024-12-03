using System.Text.Json.Serialization;

public class QuizQuestion {
  [JsonPropertyName("title")]
  required public string Title { get; set; }

  [JsonPropertyName("type")]
  required public QuizQuestionType Type { get; set; }

  [JsonPropertyName("answer")]
  required public object Answer { get; set; }

  [JsonPropertyName("choises")]
  public object[]? Choises { get; set; }
}
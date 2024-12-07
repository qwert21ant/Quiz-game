using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Quiz {
  [JsonPropertyName("info")]
  public required QuizInfo Info { get; set; }

  [JsonPropertyName("questions")]
  public List<QuizQuestion> Questions { get; set; } = new ();
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class QuizzesStorage {
  [JsonPropertyName("quizzes")]
  public Dictionary<string, Dictionary<string, Quiz>> Quizzes { get; set; } = new ();
}
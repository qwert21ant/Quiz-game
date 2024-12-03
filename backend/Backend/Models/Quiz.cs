using System.Text.Json.Serialization;

public class Quiz {
  [JsonPropertyName("name")]
  required public string Name { get; set; }

  [JsonPropertyName("questions")]
  required public QuizQuestion[] Questions { get; set; }
}
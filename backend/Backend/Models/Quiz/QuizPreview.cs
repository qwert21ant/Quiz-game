using System.Text.Json.Serialization;

public class QuizPreview {
  [JsonPropertyName("name")]
  required public string Name { get; set; }
}
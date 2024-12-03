using System.Text.Json.Serialization;

public class UserData {
  [JsonPropertyName("username")]
  required public string Username { get; set; }

  [JsonPropertyName("activeRoom")]
  public string? ActiveRoom { get; set; }

  [JsonPropertyName("quizes")]
  public QuizPreview[] Quizes { get; set; } = [];
}
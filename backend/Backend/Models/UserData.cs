using System.Text.Json.Serialization;

public class UserData {
  [JsonPropertyName("username")]
  required public string Username { get; set; }

  [JsonPropertyName("activeRoom")]
  required public string? ActiveRoom { get; set; }

  [JsonPropertyName("quizes")]
  required public QuizPreview[] Quizes { get; set; }
}
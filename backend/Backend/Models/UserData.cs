using System.Text.Json.Serialization;

public class UserData {
  [JsonPropertyName("username")]
  required public string Username { get; set; }

  [JsonPropertyName("quizzes")]
  public QuizInfo[] Quizzes { get; set; } = [];
}
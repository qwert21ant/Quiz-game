using System.Text.Json.Serialization;

public class Credentials {
  [JsonPropertyName("username")]
  required public string Username { get; set; }

  [JsonPropertyName("password")]
  required public string Password { get; set; }
}
using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RoomsStorage {
  [JsonPropertyName("rooms")]
  public Dictionary<string, Room> Rooms { get; set; } = new Dictionary<string, Room>();

  [JsonPropertyName("roomIdToUser")]
  public Dictionary<string, string> RoomIdToUser { get; set; } = new Dictionary<string, string>();
}
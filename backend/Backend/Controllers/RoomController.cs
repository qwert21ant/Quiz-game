using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("room")]
public class RoomController : Controller {
  private IRoomService roomService;

  public RoomController(IRoomService roomService) {
    this.roomService = roomService;
  }

  [HttpGet("config")]
  public async Task<IActionResult> GetConfig() {
    var user = HttpContext.User.Identity!.Name!;

    var config = await roomService.GetRoomConfig(user);
    return Json(config);
  }

  [HttpPost("update")]
  public async Task<IActionResult> UpdateConfig([FromBody] RoomConfig roomConfig) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.UpdateConfig(user, roomConfig);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState() {
    var user = HttpContext.User.Identity!.Name!;

    var state = await roomService.GetRoomState(user);
    return Json(state);
  }

  [HttpPost("open")]
  public async Task<IActionResult> OpenRoom() {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.OpenRoom(user);
    return Ok();
  }

  [HttpPost("close")]
  public async Task<IActionResult> CloseRoom() {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.CloseRoom(user);
    return Ok();
  }

  [HttpPost("kick")]
  public async Task<IActionResult> KickParticipant([FromBody] KickParticipantData data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.KickParticipant(user, data.Participant);
    return Ok();
  }

  [HttpPost("join")]
  public async Task<IActionResult> JoinRoom([FromBody] RoomJoinData data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.JoinRoom(user, data.Id);
    return Ok();
  }

  [HttpPost("leave")]
  public async Task<IActionResult> LeaveRoom([FromBody] RoomJoinData data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.LeaveRoom(user, data.Id);
    return Ok();
  }
}
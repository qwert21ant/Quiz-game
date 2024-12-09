using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("room/participant")]
public class RoomParticipantController : Controller {
  private IRoomService roomService;

  public RoomParticipantController(IRoomService roomService) {
    this.roomService = roomService;
  }

  [HttpPost("join")]
  public async Task<IActionResult> JoinRoom([FromBody] StringDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    roomService.JoinRoom(user, data.Value);
    return Ok();
  }

  [HttpPost("leave")]
  public async Task<IActionResult> LeaveRoom([FromBody] StringDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    roomService.LeaveRoom(user, data.Value);
    return Ok();
  }

  [HttpGet("info")]
  public async Task<IActionResult> GetInfo([FromQuery] StringDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    var info = roomService.GetRoomInfo(user, data.Value);
    return Json(info);
  }

  [HttpGet("isGameRunning")]
  public async Task<IActionResult> GetIsGameRunning([FromQuery] StringDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    var res = roomService.GetIsGameRunning(user, data.Value);
    return Json(new NumberDTO {
      Value = res ? 1 : 0,
    });
  }
}
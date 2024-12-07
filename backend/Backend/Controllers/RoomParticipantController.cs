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
  public async Task<IActionResult> JoinRoom([FromBody] RoomIdDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.JoinRoom(user, data.Id);
    return Ok();
  }

  [HttpPost("leave")]
  public async Task<IActionResult> LeaveRoom([FromBody] RoomIdDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.LeaveRoom(user, data.Id);
    return Ok();
  }

  [HttpGet("info")]
  public async Task<IActionResult> GetInfo([FromQuery] RoomIdDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    var info = await roomService.GetRoomInfo(user, data.Id);
    return Json(info);
  }
}
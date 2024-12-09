using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("/room/admin")]
public class RoomAdminController : Controller {
  private IRoomService roomService;

  public RoomAdminController(IRoomService roomService) {
    this.roomService = roomService;
  }

  [HttpGet("config")]
  public async Task<IActionResult> GetConfig() {
    var user = HttpContext.User.Identity!.Name!;

    var config = roomService.GetRoomConfig(user);
    return Json(config);
  }

  [HttpPost("update")]
  public async Task<IActionResult> UpdateConfig([FromBody] RoomConfig roomConfig) {
    var user = HttpContext.User.Identity!.Name!;

    roomService.UpdateConfig(user, roomConfig);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState() {
    var user = HttpContext.User.Identity!.Name!;

    var state = roomService.GetRoomState(user);
    return Json(state);
  }

  [HttpPost("open")]
  public async Task<IActionResult> OpenRoom() {
    var user = HttpContext.User.Identity!.Name!;

    roomService.OpenRoom(user);
    return Ok();
  }

  [HttpPost("close")]
  public async Task<IActionResult> CloseRoom() {
    var user = HttpContext.User.Identity!.Name!;

    roomService.CloseRoom(user);
    return Ok();
  }

  [HttpPost("kick")]
  public async Task<IActionResult> KickParticipant([FromBody] StringDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    roomService.KickParticipant(user, data.Value);
    return Ok();
  }
}
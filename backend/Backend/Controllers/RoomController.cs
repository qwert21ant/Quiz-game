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
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("/game/participant")]
public class GameParticipantController : Controller {
  private IRoomService roomService;

  public GameParticipantController(IRoomService roomService) {
    this.roomService = roomService;
  }

  [HttpPost("answer")]
  public async Task<IActionResult> Answer() {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.OpenRoom(user);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState() {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.CloseRoom(user);
    return Ok();
  }
}
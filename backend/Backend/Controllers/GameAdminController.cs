using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("/game/admin")]
public class GameAdminController : Controller {
  private IRoomService roomService;

  public GameAdminController(IRoomService roomService) {
    this.roomService = roomService;
  }

  [HttpPost("start")]
  public async Task<IActionResult> StartGame() {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.OpenRoom(user);
    return Ok();
  }

  [HttpPost("selectNextQuestion")]
  public async Task<IActionResult> SelectNextQuestion() {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.CloseRoom(user);
    return Ok();
  }

  [HttpPost("nextQuestion")]
  public async Task<IActionResult> NextQuestion([FromBody] KickParticipantDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.KickParticipant(user, data.Participant);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState([FromBody] KickParticipantDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    await roomService.KickParticipant(user, data.Participant);
    return Ok();
  }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("/game/participant")]
public class GameParticipantController : Controller {
  private IGameService gameService;

  public GameParticipantController(IGameService gameService) {
    this.gameService = gameService;
  }

  [HttpPost("answer")]
  public async Task<IActionResult> Answer([FromBody] GameAnswerDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    gameService.Answer(user, data.RoomId, data.Answer);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState([FromQuery] StringDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    var state = gameService.GetParticipantGameState(user, data.Value);
    return Json(state);
  }
}
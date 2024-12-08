using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("/game/admin")]
public class GameAdminController : Controller {
  private IGameService gameService;

  public GameAdminController(IGameService gameService) {
    this.gameService = gameService;
  }

  [HttpPost("start")]
  public async Task<IActionResult> StartGame() {
    var user = HttpContext.User.Identity!.Name!;

    await gameService.StartGame(user);
    return Ok();
  }

  [HttpPost("end")]
  public async Task<IActionResult> EndGame() {
    var user = HttpContext.User.Identity!.Name!;

    await gameService.EndGame(user);
    return Ok();
  }

  [HttpPost("selectNextQuestion")]
  public async Task<IActionResult> SelectNextQuestion([FromBody] NumberDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    await gameService.SelectNextQuestion(user, data.Value);
    return Ok();
  }

  [HttpPost("nextQuestion")]
  public async Task<IActionResult> NextQuestion() {
    var user = HttpContext.User.Identity!.Name!;

    await gameService.NextQuestion(user);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState() {
    var user = HttpContext.User.Identity!.Name!;

    var state = await gameService.GetAdminGameState(user);
    return Json(state);
  }
}
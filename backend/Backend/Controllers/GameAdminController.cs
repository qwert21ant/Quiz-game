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

    gameService.StartGame(user);
    return Ok();
  }

  [HttpPost("end")]
  public async Task<IActionResult> EndGame() {
    var user = HttpContext.User.Identity!.Name!;

    gameService.EndGame(user);
    return Ok();
  }

  [HttpPost("selectNextQuestion")]
  public async Task<IActionResult> SelectNextQuestion([FromBody] NumberDTO data) {
    var user = HttpContext.User.Identity!.Name!;

    gameService.SelectNextQuestion(user, data.Value);
    return Ok();
  }

  [HttpPost("nextQuestion")]
  public async Task<IActionResult> NextQuestion() {
    var user = HttpContext.User.Identity!.Name!;

    gameService.NextQuestion(user);
    return Ok();
  }

  [HttpPost("gotoResults")]
  public async Task<IActionResult> GoToResults() {
    var user = HttpContext.User.Identity!.Name!;

    gameService.GoToResults(user);
    return Ok();
  }

  [HttpGet("state")]
  public async Task<IActionResult> GetState() {
    var user = HttpContext.User.Identity!.Name!;

    var state = gameService.GetAdminGameState(user);
    return Json(state);
  }
}
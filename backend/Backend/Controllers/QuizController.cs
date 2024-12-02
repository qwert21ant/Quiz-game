using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("quiz")]
public class QuizController : Controller {
  [HttpPost("echo")]
  public IActionResult Echo() {
    var x = HttpContext.User.Identity?.Name;
    return Ok(x);
  }
}
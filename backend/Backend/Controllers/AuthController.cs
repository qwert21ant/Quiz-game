using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("auth")]
public class AuthController : Controller {
  private readonly ICredentialsService credsService;

  public AuthController(ICredentialsService credsService) {
    this.credsService = credsService;
  }

  [HttpGet("me")]
  public async Task<IActionResult> GetUsername() {
    var user = HttpContext.User.Identity?.Name;

    if (user == null)
      return Unauthorized();

    return Ok(user);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] Credentials creds) {
    if (credsService.Validate(creds)) {
      await LoginInner(creds.Username);
      return Ok();
    }

    return Unauthorized();
  }

  [HttpPost("signup")]
  public async Task<IActionResult> SignUp([FromBody] Credentials creds) {
    if (credsService.Exists(creds.Username))
      return BadRequest("This username is already in use");
    
    await credsService.Add(creds);
    await LoginInner(creds.Username);
    return Ok();
  }

  [HttpPost("logout")]
  public async Task<IActionResult> LogOut() {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Ok();
  }

  private async Task LoginInner(string login) {
    var id = new ClaimsIdentity([
      new Claim(ClaimsIdentity.DefaultNameClaimType, login)
    ], "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, "");

    await HttpContext.SignInAsync(
      CookieAuthenticationDefaults.AuthenticationScheme,
      new ClaimsPrincipal(id)
    );
  }
}
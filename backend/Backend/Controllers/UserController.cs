using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
public class UserController : Controller {
  private readonly ICredentialsService credsService;

  public UserController(ICredentialsService credsService) {
    this.credsService = credsService;
  }

  [Route("user")]
  public async Task<IActionResult> GetUserData() {
    var name = HttpContext.User.Identity!.Name;

    return Json(new UserData {
      Username = name ?? "???",
      ActiveRoom = null,
      Quizes = []
    });
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
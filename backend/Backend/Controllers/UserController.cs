using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
public class UserController : Controller {
  private readonly IUserService userService;

  public UserController(IUserService userService) {
    this.userService = userService;
  }

  [Route("user")]
  public async Task<IActionResult> GetUserData() {
    var name = HttpContext.User.Identity!.Name;

    if (name == null)
      throw new Exception("Username not found in HttpContext");

    return Json(await userService.GetUserData(name));
  }
}
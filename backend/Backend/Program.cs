using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

public class Program {
  public static void Main(string[] args) {
    var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
      Args = args,
      WebRootPath = "../../frontend/dist"
    });

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options => {
        options.Cookie.Name = "Auth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context => {
          context.Response.StatusCode = 401;
          return Task.CompletedTask;
        };
      });
    
    builder.Services.AddAuthorization();

    builder.Services
      .AddSingleton<ICredentialsService, CredentialsService>()
      .AddSingleton<IUserService, UserService>()
      .AddSingleton<IQuizService, QuizService>()
      .AddSingleton<IRoomService, RoomService>()
      .AddSingleton<IGameService, GameService>();

    builder.Services.AddControllers();

    var app = builder.Build();

    var rootDir = app.Services.GetRequiredService<IOptions<AppSettings>>().Value.RootDir;
    Directory.CreateDirectory(rootDir);

    if (app.Environment.IsDevelopment())
      app.UseCors(builder => builder
        .WithOrigins(["http://localhost:5089"])
        .AllowAnyHeader()
        .AllowCredentials());

    app.UseMiddleware<ServiceExceptionHandlerMiddleware>();

    app.UseRouting();
    app.UseAuthorization();

    app.MapGroup("/api")
      .MapControllers();

    app.UseStaticFiles();
    app.MapFallbackToFile("index.html");

    app.Run();
  }
}
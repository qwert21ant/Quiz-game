using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Program {
  public static void Main(string[] args) {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options => {
        // options.LoginPath = "/login";
        options.Cookie.Name = "Auth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context => {
          context.Response.StatusCode = 401;
          return Task.CompletedTask;
        };
      });
    
    builder.Services.AddAuthorization();

    builder.Services.AddSingleton<ICredentialsService, CredentialsService>();

    builder.Services.AddControllers();

    var app = builder.Build();

    app.UseCors(builder => builder
      .WithOrigins(["http://localhost:5089"])
      .AllowAnyHeader()
      .AllowCredentials());

    app.UseAuthentication();

    app.Map("/api", api => {
      api.UseRouting();

      api.UseAuthorization();

      api.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    });

    app.Run();
  }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ServiceExceptionHandlerMiddleware {
  private readonly RequestDelegate _next;

  public ServiceExceptionHandlerMiddleware(RequestDelegate next) {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context) {
    try {
      await _next(context);
    } catch (ServiceException ex) {
      context.Response.StatusCode = 400;
      await context.Response.WriteAsync(ex.Message);
    }
  }
}

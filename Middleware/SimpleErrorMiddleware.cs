using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

public class SimpleErrorMiddleware
{
    private readonly RequestDelegate _next;

    public SimpleErrorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception)
        {
            // Hata sayfasına yönlendir
            context.Response.Redirect("/Home/Error");
        }
    }
}

// Extension method
public static class SimpleErrorMiddlewareExtensions
{
    public static IApplicationBuilder UseSimpleError(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SimpleErrorMiddleware>();
    }
} 
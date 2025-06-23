namespace Northwind.Mvc.Extensions;

public static class WebApplicationExtensions
{
  public static WebApplication UseNorthwindLocalization(this WebApplication app)
  {
    string[] cultures = { "en-US", "en-GB", "fr", "fr-FR" };

    RequestLocalizationOptions localizationOptions = new();

    // cultures[0] will be "en-US"
    localizationOptions.SetDefaultCulture(cultures[0])
      // Set globalization of data formats like dates and currencies.
      .AddSupportedCultures(cultures)
      // Set localization of user interface text.
      .AddSupportedUICultures(cultures);

    app.UseRequestLocalization(localizationOptions);

    return app;
  }

  public static WebApplication UseRouteLoggerAndBonjourEndpoint(
    this WebApplication app)
  {
    // Implementing an anonymous inline delegate as middleware
    // to intercept HTTP requests and responses.
    app.Use(async (HttpContext context, Func<Task> next) =>
    {
      WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
      RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

      if (rep is not null)
      {
        WriteLine($"Endpoint: {rep.DisplayName}");
        WriteLine($"Route: {rep.RoutePattern.RawText}");
      }

      if (context.Request.Path == "/bonjour")
      {
        // In the case of a match on URL path, this becomes a terminating
        // delegate that returns so does not call the next delegate.
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
      }

      // We could modify the request before calling the next delegate.
      // Call the next delegate in the pipeline.
      await next();

      // The HTTP response is now being sent back through the pipeline.
      // We could modify the response at this point before it continues.
    });

    return app;
  }
}

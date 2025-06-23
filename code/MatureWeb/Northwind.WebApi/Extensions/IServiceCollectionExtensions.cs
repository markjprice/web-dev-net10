using Asp.Versioning; // To use ApiVersion.

namespace Northwind.WebApi.Extensions;

public static class IServiceCollectionExtensions
{
  public static IServiceCollection AddUriVersioning(this IServiceCollection services)
  {
    services.AddApiVersioning(options =>
    {
      options.DefaultApiVersion = new ApiVersion(1, 0);
      options.AssumeDefaultVersionWhenUnspecified = true;
      options.ReportApiVersions = true;

      // Use URL segment for versioning: /api/v1/customers
      options.ApiVersionReader = new UrlSegmentApiVersionReader();

      // Use query string for versioning: /api/customers?api-version=1.0
      // options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

      // Use header for versioning: X-Version: 1.0
      // options.ApiVersionReader = new HeaderApiVersionReader("X-Version");

      // Use multiple versioning schemes.
      // options.ApiVersionReader = ApiVersionReader.Combine(
      //   new QueryStringApiVersionReader("api-version"),
      //   new HeaderApiVersionReader("X-API-Version"));
    })
    .AddMvc()
    .AddApiExplorer();

    return services;
  }
}

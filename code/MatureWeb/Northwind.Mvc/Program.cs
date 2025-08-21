// To use AddIdentityDatabase and AddNorthwindDatabase methods.
using Northwind.Mvc.Extensions;
// To use StaticWebAssetsLoader.
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Northwind.Mvc; // To use DurationInSeconds.
using Microsoft.Extensions.Caching.Memory; // To use IMemoryCache and so on.
using Microsoft.Extensions.Caching.Hybrid; // To use HybridCacheEntryOptions.
using Northwind.Repositories; // To use ICustomerRepository.
using Northwind.Mvc.Options; // To use NorthwindOptions.
using Northwind.Mvc.Clients; // To use ICustomersClient.
using Refit; // To use AddRefitClient and so on.

#region Configure the host web server including services.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
  .AddRefitClient<ICustomersClient>()
  .ConfigureHttpClient(c =>
  {
    c.BaseAddress = new Uri("https://localhost:5091");
  });

builder.AddNorthwindWebApiClient();

builder.Services.Configure<NorthwindOptions>(builder
  .Configuration.GetSection("Northwind"));

builder.Services.AddHybridCache(options =>
{
  options.DefaultEntryOptions = new HybridCacheEntryOptions
  {
    Expiration = TimeSpan.FromSeconds(DurationInSeconds.OneMinute),
    LocalCacheExpiration = TimeSpan.FromSeconds(DurationInSeconds.HalfMinute)
  };
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
  new MemoryCacheOptions
  {
    TrackStatistics = true,
    SizeLimit = 50 // Products.
  }));

// Enable switching environments (Development, Production) during development.
StaticWebAssetsLoader.UseStaticWebAssets(
  builder.Environment, builder.Configuration);

builder.AddIdentityDatabase();

builder.Services.AddLocalization(
  options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
  .AddViewLocalization();

builder.AddNorthwindDatabase();

builder.Services.AddOutputCache(options =>
{
  options.DefaultExpirationTimeSpan =
    TimeSpan.FromSeconds(DurationInSeconds.TenSeconds);

  options.AddPolicy("views", p => p.SetVaryByQuery("alertstyle"));
});

builder.Services.AddScoped<ICustomerRepository,
  CustomerRepository>();

builder.AddNorthwindODataClient();

var app = builder.Build();

#endregion

#region Configure the HTTP request pipeline.

// Configure the HTTP request pipeline.

app.UseNorthwindLocalization();

if (app.Environment.IsDevelopment())
{
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseOutputCache();

app.UseRouteLoggerAndBonjourEndpoint();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}")
  .WithStaticAssets();
//  .CacheOutput(policyName: "views");

app.MapRazorPages()
  .WithStaticAssets();

app.MapGet("/notcached", () => DateTime.Now.ToString());
app.MapGet("/cached", () => DateTime.Now.ToString()).CacheOutput();

app.MapGet("/env", () =>
  $"Environment is {app.Environment.EnvironmentName}");

#endregion

#region Start the host web server listening for HTTP requests.

app.Run();

#endregion

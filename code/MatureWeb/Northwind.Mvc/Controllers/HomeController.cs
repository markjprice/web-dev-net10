using Microsoft.AspNetCore.Authorization; // To use [Authorize].
using Microsoft.AspNetCore.Mvc; // To use Controller, IActionResult.
using Microsoft.EntityFrameworkCore; // To use Include method.
using Microsoft.Extensions.Caching.Distributed; // To use IDistributedCache.
using Microsoft.Extensions.Caching.Memory; // To use IMemoryCache.
using Northwind.EntityModels; // To use NorthwindContext.
using Northwind.Mvc.Models; // To use ErrorViewModel.
using System.Diagnostics; // To use Activity.
using System.Text.Json; // To use JsonSerializer.

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private readonly NorthwindContext _db;
  private readonly IMemoryCache _memoryCache;
  private const string ProductKey = "PROD";
  private readonly IDistributedCache _distributedCache;
  private const string CategoriesKey = "CATEGORIES";
  private readonly IHttpClientFactory _clientFactory;

  public HomeController(ILogger<HomeController> logger,
    NorthwindContext db, IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    IHttpClientFactory httpClientFactory)
  {
    _logger = logger;
    _db = db;
    _memoryCache = memoryCache;
    _distributedCache = distributedCache;
    _clientFactory = httpClientFactory;
  }

  [ResponseCache(Duration = DurationInSeconds.TenSeconds,
    Location = ResponseCacheLocation.Any)]
  public async Task<IActionResult> Index()
  {
    /*
    _logger.LogError("This is a serious error (not really!)");
    _logger.LogWarning("This is your first warning!");
    _logger.LogWarning("Second warning!");
    _logger.LogInformation("I am in the Index method of the HomeController.");
    */

    HttpClient client = _clientFactory.CreateClient(
      name: "Northwind.WebApi");

    HttpRequestMessage request = new(
      method: HttpMethod.Get, requestUri: "api/countries");

    try
    {
      HttpResponseMessage response = await client.SendAsync(request);

      string[]? countries = await response.Content
        .ReadFromJsonAsync<string[]>();

      if (countries is not null)
      {
        ViewData["Countries"] = countries;
      }
      else
      {
        _logger.LogWarning("No countries were returned from the web service.");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(
        $"Exception when calling countries web service: {ex.Message}");
    }

    // Try to get the cached value.
    List<Category>? cachedValue = null;

    byte[]? cachedValueBytes =
      await _distributedCache.GetAsync(CategoriesKey);

    if (cachedValueBytes is null)
    {
      cachedValue = await GetCategoriesFromDatabaseAsync();
    }
    else
    {
      cachedValue = JsonSerializer
        .Deserialize<List<Category>>(cachedValueBytes);

      if (cachedValue is null)
      {
        cachedValue = await GetCategoriesFromDatabaseAsync();
      }
    }

    HomeIndexViewModel model = new
    (
      VisitorCount: Random.Shared.Next(1, 1001),
      Categories: cachedValue ?? new List<Category>(),
      Products: await _db.Products.ToListAsync()
    );

    return View(model); // Pass the model to the view.
  }

  [Route("private")]
  public async Task<IActionResult> Privacy()
  {
    // Construct a dictionary to store properties of a shipper.
    Dictionary<string, string>? keyValuePairs = null;

    // Find the shipper with ID of 1.
    Shipper? shipper1 = await _db.Shippers.FindAsync(1);

    if (shipper1 is not null)
    {
      keyValuePairs = new()
    {
      { "ShipperId", shipper1.ShipperId.ToString() },
      { "CompanyName", shipper1.CompanyName },
      { "Phone", shipper1.Phone ?? string.Empty }
    };
    }

    ViewData["shipper1"] = keyValuePairs;
    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }

  public async Task<IActionResult> ProductDetail(int? id,
    string alertstyle = "success")
  {
    ViewData["alertstyle"] = alertstyle;

    if (!id.HasValue)
    {
      return BadRequest("You must pass a product ID in the route, for example, /Home/ProductDetail/21");
    }

    // Try to get the cached product.
    if (!_memoryCache.TryGetValue($"{ProductKey}{id}",
       out Product? model))
    {
      // If the cached value is not found, get the value from the database.
      model = await _db.Products.Include(p => p.Category)
        .SingleOrDefaultAsync(p => p.ProductId == id);

      if (model is null)
      {
        return NotFound($"ProductId {id} not found.");
      }

      MemoryCacheEntryOptions cacheEntryOptions = new()
      {
        SlidingExpiration = TimeSpan.FromSeconds(DurationInSeconds.TenSeconds),
        Size = 1 // product
      };
      _memoryCache.Set($"{ProductKey}{id}", model, cacheEntryOptions);
    }
    MemoryCacheStatistics? stats = _memoryCache.GetCurrentStatistics();

    _logger.LogInformation($"Memory cache. Total hits: {stats?
      .TotalHits}. Estimated size: {stats?.CurrentEstimatedSize}.");

    return View(model); // Pass model to view and then return result.
  }

  // This action method will handle GET and other requests except POST.
  [Authorize(Roles = "Administrators")]
  public IActionResult ModelBinding()
  {
    return View(); // The page with a form to submit.
  }

  [HttpPost] // This action method will handle POST requests.
  public IActionResult ModelBinding(Thing thing)
  {
    HomeModelBindingViewModel model = new(
      Thing: thing, HasErrors: !ModelState.IsValid,
      ValidationErrors: ModelState.Values
        .SelectMany(state => state.Errors)
        .Select(error => error.ErrorMessage)
    );
    return View(model); // Show the model bound thing.
  }

  public IActionResult ProductsThatCostMoreThan(decimal? price)
  {
    if (!price.HasValue)
    {
      return BadRequest("You must pass a product price in the query string, for example, /Home/ProductsThatCostMoreThan?price=50");
    }

    IEnumerable<Product> model = _db.Products
      .Include(p => p.Category)
      .Include(p => p.Supplier)
      .Where(p => p.UnitPrice > price);

    if (!model.Any())
    {
      return NotFound(
        $"No products cost more than {price:C}.");
    }

    // Format currency using web server's culture.
    ViewData["MaxPrice"] = price.Value.ToString("C");

    // We can override the search path convention.
    return View("Views/Home/CostlyProducts.cshtml", model);
  }

  public async Task<IActionResult> CategoryDetail(int? id)
  {
    if (!id.HasValue)
    {
      return BadRequest("You must pass a category ID in the route, for example, /Home/CategoryDetail/6");
    }
    Category? model = await _db.Categories.Include(p => p.Products)
      .SingleOrDefaultAsync(p => p.CategoryId == id);
    if (model is null)
    {
      return NotFound($"CategoryId {id} not found.");
    }
    return View(model); // Pass model to view and then return result.
  }

  public IActionResult Orders(
    string? id = null, string? country = null)
  {
    // Start with a simplified initial model.
    IEnumerable<Order> model = _db.Orders
      .Include(order => order.Customer)
      .Include(order => order.OrderDetails);

    // Add filtering based on parameters.
    if (id is not null)
    {
      model = model.Where(order => order.Customer?.CustomerId == id);
    }
    else if (country is not null)
    {
      model = model.Where(order => order.Customer?.Country == country);
    }

    // Add ordering and make enumerable.
    model = model
      .OrderByDescending(order => order.OrderDetails
        .Sum(detail => detail.Quantity * detail.UnitPrice))
      .AsEnumerable();

    return View(model);
  }

  public IActionResult Shipper(Shipper shipper)
  {
    return View(shipper);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult ProcessShipper(Shipper shipper)
  {
    return Json(shipper);
  }

  private async Task<List<Category>> GetCategoriesFromDatabaseAsync()
  {
    List<Category> cachedValue = await _db.Categories.ToListAsync();

    DistributedCacheEntryOptions cacheEntryOptions = new()
    {
      // Allow readers to reset the cache entry's lifetime.
      SlidingExpiration = TimeSpan.FromMinutes(1),
      // Set an absolute expiration time for the cache entry.
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
    };

    byte[]? cachedValueBytes =
      JsonSerializer.SerializeToUtf8Bytes(cachedValue);

    await _distributedCache.SetAsync(CategoriesKey,
      cachedValueBytes, cacheEntryOptions);

    return cachedValue;
  }

  public async Task<IActionResult> Customers(string country)
  {
    string uri;

    if (string.IsNullOrEmpty(country))
    {
      ViewData["Title"] = "All Customers Worldwide";
      uri = "api/v1/customers";
    }
    else
    {
      ViewData["Title"] = $"Customers in {country}";
      uri = $"api/v1/customers/?country={country}";
    }

    HttpClient client = _clientFactory.CreateClient(
      name: "Northwind.WebApi");

    HttpRequestMessage request = new(
      method: HttpMethod.Get, requestUri: uri);

    HttpResponseMessage response = await client.SendAsync(request);

    IEnumerable<Customer>? model = await response.Content
      .ReadFromJsonAsync<IEnumerable<Customer>>();

    return View(model);
  }
}

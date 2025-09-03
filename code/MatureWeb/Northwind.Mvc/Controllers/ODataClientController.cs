using Microsoft.AspNetCore.Mvc; // To use Controller.
using Northwind.EntityModels; // To use Product.
using Northwind.Mvc.Models; // To use ODataProducts.

namespace Northwind.Mvc.Controllers;

public class ODataClientController : Controller
{
  private readonly ILogger<ODataClientController> _logger;
  private readonly IHttpClientFactory _httpClientFactory;

  public ODataClientController(
    ILogger<ODataClientController> logger,
    IHttpClientFactory httpClientFactory)
  {
    _logger = logger;
    _httpClientFactory = httpClientFactory;
  }

  public async Task<IActionResult> Index(string startsWith = "Cha")
  {
    IEnumerable<Product>? model = Enumerable.Empty<Product>();

    try
    {
      HttpClient client = _httpClientFactory.CreateClient(
        name: "Northwind.OData");

      HttpRequestMessage request = new(
        method: HttpMethod.Get, requestUri:
        "catalog/products/?$filter=startswith(ProductName," +
        $"'{startsWith}')&$select=ProductId,ProductName,UnitPrice");

      HttpResponseMessage response = await client.SendAsync(request);

      ViewData["startsWith"] = startsWith;

      model = (await response.Content
        .ReadFromJsonAsync<ODataProducts>())?.Value;
    }
    catch (Exception ex)
    {
      _logger.LogWarning(
        $"Northwind.OData exception: {ex.Message}");
    }
    return View(model);
  }
}

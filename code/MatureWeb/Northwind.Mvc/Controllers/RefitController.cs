using Microsoft.AspNetCore.Mvc; // To use Controller and IActionResult.
using Northwind.EntityModels; // To use Customer.
using Northwind.Mvc.Clients; // To use ICustomersClient.

namespace Northwind.Mvc.Controllers;

public class RefitController : Controller
{
  public async Task<IActionResult> Index([FromServices] ICustomersClient client)
  {
    List<Customer> model = await client.GetCustomersAsync();

    // Reuse the same view as the CustomersController.
    return View("Views/Customers/Index.cshtml", model);
  }
}

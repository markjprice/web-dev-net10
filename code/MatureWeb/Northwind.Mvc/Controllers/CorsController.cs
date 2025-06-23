using Microsoft.AspNetCore.Mvc;

namespace Northwind.Mvc.Controllers;

public class CorsController : Controller
{
  public IActionResult JavaScript()
  {
    return View();
  }
}

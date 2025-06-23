using Microsoft.AspNetCore.Mvc; // To use [Route], [ApiController], ControllerBase and so on.
using Microsoft.EntityFrameworkCore; // To use ToArrayAsync.
using Northwind.EntityModels; // To use Customer and NorthwindContext.

namespace Northwind.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
  // GET: api/countries
  // This will always return an array of country names (but it might be empty).
  [HttpGet]
  [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
  public async Task<string?[]> GetCountries([FromServices] NorthwindContext db)
  {
      return await db.Customers
      .Select(customer => customer.Country)
      .Distinct()
      .Order()
      .ToArrayAsync();
  }
}

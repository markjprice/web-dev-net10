// To use [Route], [ApiController], ControllerBase and so on.
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Northwind.EntityModels; // To use Customer.
using Northwind.Repositories; // To use ICustomerRepository.

namespace Northwind.WebApi.Controllers;

[ApiVersion("2.0")]
// Base address: api/v2/customers
[Route("api/v{version:apiVersion}/customers")]
[ApiController]
public class CustomersV2Controller : ControllerBase
{
  private readonly ICustomerRepository _repo;

  // Constructor injects repository registered in Program.cs.
  public CustomersV2Controller(ICustomerRepository repo)
  {
    _repo = repo;
  }

  // GET: api/customers
  // GET: api/customers/?country=[country]
  // this will always return a list of customers (but it might be empty)
  [HttpGet]
  [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
  /// <summary>
  /// Gets customers. Optionally, filter by country.
  /// </summary>
  /// <remarks>
  /// country parameter is case-sensitive. Use USA or Germany, not usa or Germany!
  /// </remarks>
  /// <param name="country">The name of the country to filter by.</param>
  /// <returns>An array of customers in JSON (default) or XML.</returns>
  public async Task<IEnumerable<Customer>> GetCustomers(string? country)
  {
    if (string.IsNullOrWhiteSpace(country))
    {
      return await _repo.RetrieveAllAsync();
    }
    else
    {
      return (await _repo.RetrieveAllAsync())
        .Where(customer => customer.Country == country);
    }
  }

  // GET: api/customers/[id]
  [HttpGet("{id}", Name = nameof(GetCustomer))] // Named route.
  [ProducesResponseType(200, Type = typeof(Customer))]
  [ProducesResponseType(404)]
  [ResponseCache(Duration = 5, // Cache-Control: max-age=5
    Location = ResponseCacheLocation.Any, // Cache-Control: public
    VaryByHeader = "User-Agent" // Vary: User-Agent
  )]
  public async Task<IActionResult> GetCustomer(string id)
  {
    Customer? c = await _repo.RetrieveAsync(id, default);

    if (c == null)
    {
      return NotFound(); // 404 Resource not found.
    }

    c.CompanyName += " (v2)"; // Add v2 to CompanyName.

    return Ok(c); // 200 OK with customer in body
  }

  // POST: api/customers
  // BODY: Customer (JSON, XML)
  [HttpPost]
  [ProducesResponseType(201, Type = typeof(Customer))]
  [ProducesResponseType(400)]
  public async Task<IActionResult> Create([FromBody] Customer c)
  {
    if (c == null)
    {
      return BadRequest(); // 400 Bad request.
    }
    Customer? addedCustomer = await _repo.CreateAsync(c);
    if (addedCustomer == null)
    {
      return BadRequest("Repository failed to create customer.");
    }
    else
    {
      return CreatedAtRoute( // 201 Created.
        routeName: nameof(GetCustomer),
        routeValues: new { id = addedCustomer.CustomerId.ToLower() },
        value: addedCustomer);
    }
  }

  // PUT: api/customers/[id]
  // BODY: Customer (JSON, XML)
  [HttpPut("{id}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(400)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> Update(
    string id, [FromBody] Customer c)
  {
    id = id.ToUpper();
    c.CustomerId = c.CustomerId.ToUpper();

    if (c == null || c.CustomerId != id)
    {
      return BadRequest(); // 400 Bad request.
    }

    Customer? existing = await _repo.RetrieveAsync(id, default);
    if (existing == null)
    {
      return NotFound(); // 404 Resource not found.
    }

    await _repo.UpdateAsync(c);

    return new NoContentResult(); // 204 No content.
  }

  // DELETE: api/customers/[id]
  [HttpDelete("{id}")]
  [ProducesResponseType(204)]
  [ProducesResponseType(400)]
  [ProducesResponseType(404)]
  public async Task<IActionResult> Delete(string id)
  {
    // Take control of problem details.
    if (id == "bad")
    {
      ProblemDetails problemDetails = new()
      {
        Status = StatusCodes.Status400BadRequest,
        Type = "https://localhost:5091/customers/failed-to-delete",
        Title = $"Customer ID {id} found but failed to delete.",
        Detail = "More details like Company Name, Country and so on.",
        Instance = HttpContext.Request.Path
      };
      return BadRequest(problemDetails); // 400 Bad Request
    }

    Customer? existing = await _repo.RetrieveAsync(id, default);

    if (existing == null)
    {
      return NotFound(); // 404 Resource not found.
    }

    bool? deleted = await _repo.DeleteAsync(id);

    if (deleted.HasValue && deleted.Value) // Short circuit AND.
    {
      return new NoContentResult(); // 204 No content.
    }
    else
    {
      return BadRequest( // 400 Bad request.
        $"Customer {id} was found but failed to delete.");
    }
  }
}

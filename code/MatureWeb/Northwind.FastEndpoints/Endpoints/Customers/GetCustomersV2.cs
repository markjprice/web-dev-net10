using FastEndpoints; // To use Endpoint<T, T>.
using Microsoft.EntityFrameworkCore; // To use ToArrayAsync.
using Northwind.EntityModels;
using Northwind.FastEndpoints.DTOs.Customers;

namespace Northwind.FastEndpoints.Endpoints.Customers;

public class GetCustomersV2 : Endpoint<CustomersRequest, CustomersResponseV2[]>
{
  private readonly NorthwindContext _db;

  public GetCustomersV2(NorthwindContext db) => _db = db;

  public override void Configure()
  {
    Verbs(Http.GET);
    Routes("/customers");
    Version(2); // <-- Version 2
    AllowAnonymous();
  }

  public override async Task HandleAsync(CustomersRequest req, CancellationToken ct)
  {
    var query = _db.Customers.AsQueryable();

    if (!string.IsNullOrWhiteSpace(req.Country))
    {
      query = query.Where(c => c.Country == req.Country);
    }

    var result = await query
      .Select(c => new CustomersResponseV2(
        c.CustomerId,
        c.CompanyName,
        c.ContactName ?? string.Empty,
        c.Country ?? string.Empty))
      .ToArrayAsync(ct);

    await Send.OkAsync(result, cancellation: ct);
  }
}

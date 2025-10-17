using FastEndpoints; // To use Endpoint<TRequest, TResponse>.
using Northwind.EntityModels; // To use Customer.

namespace Northwind.FastEndpoints.Endpoints;

public class ReplaceCustomerEndpoint : Endpoint<Customer, Customer>
{
  private readonly NorthwindContext _db;

  public ReplaceCustomerEndpoint(NorthwindContext db) => _db = db;

  public override void Configure()
  {
    Verbs(Http.PUT);
    Routes("/customers/{Id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    Customer req, CancellationToken ct)
  {
    Customer? customer = await _db.Customers.FindAsync([ req.CustomerId ], ct);

    if (customer is null)
    {
      await Send.NotFoundAsync(ct);
      return;
    }

    customer.CompanyName = req.CompanyName;
    customer.ContactName = req.ContactName;
    customer.ContactTitle = req.ContactTitle;
    customer.Address = req.Address;
    customer.City = req.City;
    customer.Region = req.Region;
    customer.PostalCode = req.PostalCode;
    customer.Country = req.Country;
    customer.Phone = req.Phone;
    customer.Fax = req.Fax;

    await _db.SaveChangesAsync(ct);
    await Send.NoContentAsync(ct);
  }
}

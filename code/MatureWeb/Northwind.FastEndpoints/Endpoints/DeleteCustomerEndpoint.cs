using FastEndpoints; // To use EndpointWithoutRequest.
using Northwind.EntityModels; // To use Customer.

namespace Northwind.FastEndpoints.Endpoints;

public class DeleteCustomerEndpoint : EndpointWithoutRequest
{
  private readonly NorthwindContext _db;

  public DeleteCustomerEndpoint(NorthwindContext db) => _db = db;

  public override void Configure()
  {
    Verbs(Http.DELETE);
    Routes("/customers/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    string? id = Route<string>("id");

    Customer? customer = await _db.Customers.FindAsync(
      [ id ], cancellationToken: ct);

    if (customer is null)
    {
      await Send.NotFoundAsync(cancellation: ct);
      return;
    }

    _db.Customers.Remove(customer);
    await _db.SaveChangesAsync(ct);

    await Send.NoContentAsync(cancellation: ct);
  }
}

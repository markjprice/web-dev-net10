using FastEndpoints; // To use Endpoint<TRequest>.
using Microsoft.AspNetCore.JsonPatch; // To use JsonPatchDocument<T>.
using Northwind.EntityModels; // To use Customer.

public class UpdateCustomerEndpoint : Endpoint<JsonPatchDocument<Customer>>
{
  private readonly NorthwindContext _db;

  public UpdateCustomerEndpoint(NorthwindContext db) => _db = db;

  public override void Configure()
  {
    Verbs(Http.PATCH);
    Routes("/customers/{id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    JsonPatchDocument<Customer> patchDoc, CancellationToken ct)
  {
    string? id = Route<string>("id");

    // Log the patch operations to console.
    patchDoc.Operations.ForEach(op =>
      WriteLine($"{op.op}: {op.path} => {op.value}"));

    var customer = await _db.Customers.FindAsync([ id ], ct);
    if (customer is null)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    patchDoc.ApplyTo(customer);

    await _db.SaveChangesAsync(ct);

    await SendNoContentAsync(ct);
  }
}

﻿using FastEndpoints; // To use Endpoint<TRequest, TResponse>.
using FluentValidation.Results; // To use ValidationResult.
using Northwind.EntityModels; // To use Customer.
using Northwind.FastEndpoints.Validators; // To use CreateCustomerValidator.

namespace Northwind.FastEndpoints.Endpoints;

public class CreateCustomerEndpoint : Endpoint<Customer, Customer>
{
  private readonly NorthwindContext _db;

  public CreateCustomerEndpoint(NorthwindContext db) => _db = db;

  public override void Configure()
  {
    Verbs(Http.POST);
    Routes("/customers");
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    Customer request, CancellationToken ct)
  {
    CreateCustomerValidator validator = new();
    ValidationResult? validationResult = await validator.ValidateAsync(request, ct);
    
    if (!validationResult.IsValid)
    {
      await SendErrorsAsync(cancellation: ct);
      return;
    }

    _db.Customers.Add(request);
    await _db.SaveChangesAsync(ct);

    await SendAsync(request, statusCode: 201, cancellation: ct);
  }
}

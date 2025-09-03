using FastEndpoints; // To use Validator<T>.
using FluentValidation; // To use NotEmpty, MaximumLength, and so on.
using Northwind.EntityModels; // To use Customer.

namespace Northwind.FastEndpoints.Validators;

public class CreateCustomerValidator : Validator<Customer>
{
  public CreateCustomerValidator()
  {
    RuleFor(x => x.CustomerId)
      .NotEmpty().WithMessage("Customer ID is required.")
      .MaximumLength(5).WithMessage("Customer ID must be exactly 5 characters long.")
      .Matches(@"^[A-Z0-9]{5}$").WithMessage("Customer ID must consist of 5 uppercase letters or digits.");
    
    RuleFor(x => x.CompanyName)
      .NotEmpty().WithMessage("Company name is required.")
      .MaximumLength(40).WithMessage("Company name must be at most 40 characters long.");

    RuleFor(x => x.ContactName)
      .NotEmpty().WithMessage("Contact name is required.")
      .MaximumLength(30).WithMessage("Contact name must be at most 30 characters long.");
    
    RuleFor(x => x.ContactTitle)
      .MaximumLength(30).WithMessage("Contact title must be at most 30 characters long.");
    
    RuleFor(x => x.Country)
      .NotEmpty().WithMessage("Country is required.")
      .MaximumLength(15).WithMessage("Country must be at most 15 characters long.");
  }
}

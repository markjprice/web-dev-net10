﻿using Microsoft.AspNetCore.Mvc; // To use ProblemDetails.
using Northwind.EntityModels; // To use Customer.
using Northwind.Repositories; // To use ICustomerRepository.

namespace Northwind.Mvc.Controllers;

public class CustomersController : Controller
{
  private readonly ICustomerRepository _repo;

  public CustomersController(ICustomerRepository customerRepository)
  {
    _repo = customerRepository;
  }

  [Route("Customers/{country?}")]
  public async Task<IActionResult> Index(
    string? country = null)
  {
    IEnumerable<Customer> model = await _repo.RetrieveAllAsync();

    if (!string.IsNullOrWhiteSpace(country))
    {
      model = model.Where(customer => customer.Country == country);
    }

    return View(model);
  }

  [Route("Customers/Detail/{id}")]
  public async Task<IActionResult> Detail(string id)
  {
    Customer? model = await _repo.RetrieveAsync(id, default);

    return View(model);
  }
}

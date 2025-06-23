using Northwind.EntityModels; // To use Customer.
using Refit; // To use [Get] attribute.

namespace Northwind.Mvc.Clients;

public interface ICustomersClient
{
  [Get("/api/v1/customers")]
  Task<List<Customer>> GetCustomersAsync();

  [Get("/api/v1/customers?country={country}")]
  Task<List<Customer>> GetCustomersAsync(string? country);

  [Get("/api/v1/customers/{id}")]
  Task<List<Customer>> GetCustomerAsync(string id);

  [Post("/api/v1/customers")]
  Task<Customer> CreateCustomerAsync([Body] Customer customer);

  [Put("/api/v1/customers/{id}")]
  Task UpdateCustomerAsync(string id, [Body] Customer customer);

  [Delete("/api/v1/customers/{id}")]
  Task DeleteCustomerAsync(string id);
}

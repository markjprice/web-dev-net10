using Northwind.EntityModels; // To use Supplier.

namespace Northwind.Mvc.Models;

public record SupplierViewModel(
  int EntitiesAffected, Supplier? Supplier);

using Northwind.EntityModels; // To use Supplier.

namespace Northwind.Mvc.Models;

public record SuppliersIndexViewModel(IEnumerable<Supplier>? Suppliers);

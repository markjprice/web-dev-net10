using Microsoft.AspNetCore.OData; // To use AddOData.
using Microsoft.OData.Edm; // To use IEdmModel.
using Microsoft.OData.ModelBuilder; // ODataConventionModelBuilder
using Northwind.EntityModels; // To use AddNorthwindContext.

namespace Northwind.OData.Extensions;

public static class IServiceCollectionExtensions
{
  private static IEdmModel GetEdmModelForCatalog()
  {
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Category>("Categories");
    builder.EntitySet<Product>("Products");
    builder.EntitySet<Supplier>("Suppliers");
    return builder.GetEdmModel();
  }

  private static IEdmModel GetEdmModelForOrderSystem()
  {
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Customer>("Customers");
    builder.EntitySet<Order>("Orders");
    builder.EntitySet<Employee>("Employees");
    builder.EntitySet<Product>("Products");
    builder.EntitySet<Shipper>("Shippers");
    return builder.GetEdmModel();
  }

  public static IServiceCollection AddNorthwindODataControllers(this IServiceCollection services)
  {
    services.AddNorthwindContext();

    services.AddControllers()
      // Register OData models.
      .AddOData(options => options

        // GET /catalog and /catalog/$metadata
        .AddRouteComponents(routePrefix: "catalog",
          model: GetEdmModelForCatalog())
      
        // GET /ordersystem and /ordersystem/$metadata
        .AddRouteComponents(routePrefix: "ordersystem",
          model: GetEdmModelForOrderSystem())

        // GET /catalog/v1, /catalog/v2, and so on.
        .AddRouteComponents(routePrefix: "catalog/v{version}",
          model: GetEdmModelForCatalog())

        // Enable query options:
        .Select() // $select for projection
        .Expand() // $expand to navigate to related entities
        .Filter() // $filter
        .OrderBy() // $orderby
        .SetMaxTop(100) // $top
        .Count() // $count
      );

    return services;
  }
}

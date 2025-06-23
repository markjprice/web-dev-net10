#region Import namespaces.

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Data;
using Northwind.EntityModels; // To use AddNorthwindContext method.
using Microsoft.Data.SqlClient; // To use SqlConnectionStringBuilder.
using System.Net.Http.Headers; // To use MediaTypeWithQualityHeaderValue.

#endregion

namespace Northwind.Mvc.Extensions;

public static class WebApplicationBuilderExtensions
{
  public static WebApplicationBuilder AddIdentityDatabase(this WebApplicationBuilder builder)
  {
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>() // Enable role management.
        .AddEntityFrameworkStores<ApplicationDbContext>();

    return builder;
  }

  public static WebApplicationBuilder AddNorthwindDatabase(this WebApplicationBuilder builder)
  {
    string? sqlServerConnection = builder.Configuration
      .GetConnectionString("NorthwindConnection");

    if (sqlServerConnection is null)
    {
      WriteLine("Northwind database connection string is missing from configuration!");
    }
    else
    {
      // If you are using SQL Server authentication then disable
      // Windows Integrated authentication and set user and password.
      SqlConnectionStringBuilder sql = new(sqlServerConnection);

      sql.IntegratedSecurity = false;
      sql.UserID = Environment.GetEnvironmentVariable("MY_SQL_USR");
      sql.Password = Environment.GetEnvironmentVariable("MY_SQL_PWD");

      builder.Services.AddNorthwindContext(sql.ConnectionString);
    }

    return builder;
  }

  public static WebApplicationBuilder AddNorthwindWebApiClient(this WebApplicationBuilder builder)
  {
    builder.Services.AddHttpClient(name: "Northwind.WebApi",
      configureClient: options =>
      {
        options.BaseAddress = new Uri("https://localhost:5091/");
        options.DefaultRequestHeaders.Accept.Add(
          new MediaTypeWithQualityHeaderValue(
          mediaType: "application/json", quality: 1.0));
      });

    return builder;
  }

  public static WebApplicationBuilder AddNorthwindODataClient(this WebApplicationBuilder builder)
  {
    builder.Services.AddHttpClient(name: "Northwind.OData",
      configureClient: options =>
      {
        options.BaseAddress = new Uri("https://localhost:5121/");
        options.DefaultRequestHeaders.Accept.Add(
          new MediaTypeWithQualityHeaderValue(
          mediaType: "application/json", quality: 1.0));
      });

    return builder;
  }
}

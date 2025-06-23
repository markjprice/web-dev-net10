using Microsoft.AspNetCore.Mvc; // To use Controller, IActionResult.
using Northwind.EntityModels; // To use NorthwindContext.
using Northwind.Mvc.Models; // To use SuppliersIndexViewModel.

namespace Northwind.Mvc.Controllers;

public class SuppliersController : Controller
{
  private readonly NorthwindContext _db;

  public SuppliersController(NorthwindContext db)
  {
    _db = db;
  }

  public IActionResult Index()
  {
    SuppliersIndexViewModel model = new(_db.Suppliers
      .OrderBy(c => c.Country)
      .ThenBy(c => c.CompanyName));

    return View(model);
  }

  // GET: /suppliers/edit/{id}
  public IActionResult Edit(int? id)
  {
    Supplier? supplierInDb = _db.Suppliers.Find(id);

    SupplierViewModel model = new(
      supplierInDb is null ? 0 : 1, supplierInDb);

    // Views\Suppliers\Edit.cshtml
    return View(model);
  }

  // POST: /suppliers/edit
  // Body: Supplier
  // Updates an existing supplier.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Edit(Supplier supplier)
  {
    int affected = 0;

    if (ModelState.IsValid)
    {
      Supplier? supplierInDb = _db.Suppliers.Find(supplier.SupplierId);

      if (supplierInDb is not null)
      {
        supplierInDb.CompanyName = supplier.CompanyName;
        supplierInDb.Country = supplier.Country;
        supplierInDb.Phone = supplier.Phone;
        /*
        // Other properties not in the HTML form.
        supplierInDb.ContactName = supplier.ContactName;
        supplierInDb.ContactTitle = supplier.ContactTitle;
        supplierInDb.Address = supplier.Address;
        supplierInDb.City = supplier.City;
        supplierInDb.Region = supplier.Region;
        supplierInDb.PostalCode = supplier.PostalCode;
        supplierInDb.Fax = supplier.Fax;
        */
        affected = _db.SaveChanges();
      }
    }

    SupplierViewModel model = new(
      affected, supplier);

    if (affected == 0) // Supplier was not updated.
    {
      // Views\Suppliers\Edit.cshtml
      return View(model);
    }
    else // Supplier was updated; show in table.
    {
      return RedirectToAction("Index");
    }
  }

  // GET: /suppliers/delete/{id}
  public IActionResult Delete(int? id)
  {
    Supplier? supplierInDb = _db.Suppliers.Find(id);

    SupplierViewModel model = new(
      supplierInDb is null ? 0 : 1, supplierInDb);

    // Views\Suppliers\Delete.cshtml
    return View(model);
  }

  // POST: /suppliers/delete/{id}
  // Removes an existing supplier.
  [HttpPost("/suppliers/delete/{id:int?}")]
  [ValidateAntiForgeryToken]
  // C# won't allow two methods with the same name and signature.
  public IActionResult DoTheDelete(int? id)
  {
    int affected = 0;

    Supplier? supplierInDb = _db.Suppliers.Find(id);

    if (supplierInDb is not null)
    {
      _db.Suppliers.Remove(supplierInDb);
      affected = _db.SaveChanges();
    }

    SupplierViewModel model = new(
      affected, supplierInDb);

    if (affected == 0) // Supplier was not deleted.
    {
      // Views\Suppliers\Delete.cshtml
      return View(model);
    }
    else
    {
      return RedirectToAction("Index");
    }
  }

  // GET: /suppliers/add
  public IActionResult Add()
  {
    SupplierViewModel model = new(
      0, new Supplier());

    // Views\Suppliers\Add.cshtml
    return View(model);
  }

  // POST: /suppliers/add
  // Body: Supplier
  // Inserts a new supplier.
  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Add(Supplier supplier)
  {
    int affected = 0;

    if (ModelState.IsValid)
    {
      _db.Suppliers.Add(supplier);
      affected = _db.SaveChanges();
    }

    SupplierViewModel model = new(
      affected, supplier);

    if (affected == 0) // Supplier was not added.
    {
      // Views\Home\Add.cshtml
      return View(model);
    }
    else
    {
      return RedirectToAction("Index");
    }
  }
}

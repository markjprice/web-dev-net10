**Errata** (3 items)

If you find any mistakes, then please [raise an issue in this repository](https://github.com/markjprice/web-dev-net10/issues) or email me at markjprice (at) gmail.com.

> **Warning!** Avoid copying and pasting links that break over multiple lines and include hyphens or dashes because your PDF reader might remove a hyphen thinking that it being helpful but break the link! Just click on the links and they will work. Or carefully check that your PDF reader has not removed a hyphen after pasting into your web browser address bar. [See an example of this issue here](https://github.com/markjprice/cs13net9/issues/77).

- [Page 23 - Configuring CPM for this book’s projects](#page-23---configuring-cpm-for-this-books-projects)
- [Page 224 - Enabling role management and creating a role programmatically](#page-224---enabling-role-management-and-creating-a-role-programmatically)
- [Page 295 - Testing page navigation and title verification](#page-295---testing-page-navigation-and-title-verification)
- [Page 298 - Filling in input boxes and clicking elements](#page-298---filling-in-input-boxes-and-clicking-elements)


# Page 23 - Configuring CPM for this book’s projects

> Thanks to [zkazz](https://github.com/zkazz) for raising [this issue on January 16, 2026](https://github.com/markjprice/web-dev-net10/issues/1).

In Step 4, the package version entry for `Refit.HttpClientFactory` is missing a close-double-quote character at the end of the version number:
```xml
<PackageVersion Include="Refit.HttpClientFactory"
                Version="8.0.0/>
```

It should be:
```xml
<PackageVersion Include="Refit.HttpClientFactory"
                Version="8.0.0" />
```

# Page 224 - Enabling role management and creating a role programmatically

> Thanks to [zkazz](https://github.com/zkazz) for raising [this issue on January 26, 2026](https://github.com/markjprice/web-dev-net10/issues/3).

In Step 2, in the `RolesController.cs` file, I added an extra close brace near the end of an `if` statement that causes a compile error, as shown in the following code:

```cs
if (user == null)
{
  user = new();
  user.UserName = UserEmail;
  user.Email = UserEmail;

  IdentityResult result = await _userManager.CreateAsync(
    user, "Pa$$w0rd");

  LogIdentityResult(result,
    $"User {user.UserName} created successfully."); } // <-- Delete this close brace.
}
```

Deleting the extra close brace removes the compile error.

# Page 295 - Testing page navigation and title verification

> Thanks to [zkazz](https://github.com/zkazz) for raising [this issue on January 29, 2026](https://github.com/markjprice/web-dev-net10/issues/4).

In Step 7, the code for the `CleanUpSession` method ends with a `b` which prevents compilation. Delete the `b` to fix this:
https://github.com/markjprice/web-dev-net10/blob/main/code/MatureWeb/Northwind.WebUITests/MvcWebUITests.cs#L47

Also in Step 7, in the `HomePage_Title` method, the statement `await GotoHomePage(playwright);` should be `await GotoHomePage();`
https://github.com/markjprice/web-dev-net10/blob/main/code/MatureWeb/Northwind.WebUITests/MvcWebUITests.cs#L53

On page 296, Step 11 says, "in the `GotoHomePage` method", but it should say, "in the `InitializeAsync` method".
https://github.com/markjprice/web-dev-net10/blob/main/code/MatureWeb/Northwind.WebUITests/MvcWebUITests.cs#L22

# Page 298 - Filling in input boxes and clicking elements

> Thanks to [zkazz](https://github.com/zkazz) for raising [this issue on January 30, 2026](https://github.com/markjprice/web-dev-net10/issues/5).

In Step 4, in the `HomePage_VisitorCount` method, the statement `await GotoHomePage(playwright);` should be `await GotoHomePage();`, as shown in the following code:
https://github.com/markjprice/web-dev-net10/blob/main/code/MatureWeb/Northwind.WebUITests/MvcWebUITests.cs#L88

On page 299, in Step 5, in the `HomePage_FilterProducts` method, the statement `await GotoHomePage(playwright);` should be `await GotoHomePage();`, as shown in the following code:
https://github.com/markjprice/web-dev-net10/blob/main/code/MatureWeb/Northwind.WebUITests/MvcWebUITests.cs#L117

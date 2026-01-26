**Errata** (2 items)

If you find any mistakes, then please [raise an issue in this repository](https://github.com/markjprice/web-dev-net10/issues) or email me at markjprice (at) gmail.com.

> **Warning!** Avoid copying and pasting links that break over multiple lines and include hyphens or dashes because your PDF reader might remove a hyphen thinking that it being helpful but break the link! Just click on the links and they will work. Or carefully check that your PDF reader has not removed a hyphen after pasting into your web browser address bar. [See an example of this issue here](https://github.com/markjprice/cs13net9/issues/77).

- [Page 23 - Configuring CPM for this book’s projects](#page-23---configuring-cpm-for-this-books-projects)
- [Page 224 - Enabling role management and creating a role programmatically](#page-224---enabling-role-management-and-creating-a-role-programmatically)


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

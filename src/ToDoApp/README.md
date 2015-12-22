# Azure DocumentDb ToDoApp

A port of existing Azure sample app to ASP.NET5

Note: There is a real stopper to make it work:
[Missing BCrypt.dll out of the box on .NET 5 + Mono + Linux](https://social.msdn.microsoft.com/Forums/azure/en-US/a4a80fde-5282-480a-b981-2bf5bb5f64c9/missing-bcryptdll-out-of-the-box-on-net-5-mono-linux?forum=AzureDocumentDB)

## Access configuration

The Azure DocumentDb access configuration is based on layered options, as discussed in:

[Getting and setting configuration settings](https://docs.asp.net/en/latest/fundamentals/configuration.html?highlight=options#getting-and-setting-configuration-settings)
[Safe Storage of Application Secrets](https://docs.asp.net/en/latest/security/app-secrets.html?highlight=user%20secrets#safe-storage-of-application-secrets)

Configuration for Azure DocumentDb is layered in the following order:

- `appsettings.json`
- `appsettings.{development}.json`
- user secrets
- environment variables

This provides flexible and secure way to configure access to DocumentDb. `AuthKey` and `EndPoint` values are not stored in application code.
You could either:
- modify `appsettings.json` and add correct values or:
- set `Azure:DocumentDb:AuthKey` and `Azure:DocumentDb:EndPoint` with `user-secret` tool or:
- add them as environment variables when starting application.

The landing page displays (via `@inject` feature) values of Azure DocumentDb configuration options.

```html
<dl>
	<dt>Database:</dt>
	<dd>@config.Value.Database</dd>
	<dt>Collection:</dt>
	<dd>@config.Value.Collection</dd>
	<dt>EndPoint:</dt>
	<dd>@config.Value.EndPoint</dd>
	<dt>AuthKey:</dt>
	<dd>@config.Value.AuthKey</dd>
</dl>
```

## Author

@peterblazejewicz
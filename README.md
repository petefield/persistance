# pefi.persistence

A reusable .NET class library that provides a clean abstraction over MongoDB for PeFi projects. It wraps the MongoDB .NET driver behind a simple generic interface, supports dependency injection, and includes OpenTelemetry-compatible distributed tracing out of the box.

## Requirements

- .NET 9.0
- A running MongoDB instance

## Installation

```
dotnet add package pefi.persistence
```

or add a package reference directly:

```xml
<PackageReference Include="pefi.persistence" Version="*" />
```

## Usage

Register the persistence layer in your `Program.cs` or `Startup.cs`:

```csharp
// Option 1: direct connection string
builder.Services.AddPeFiPersistance("mongodb://localhost:27017");

// Option 2: fluent configuration
builder.Services.AddPeFiPersistance(options =>
{
    options.ConnectionString = "mongodb://localhost:27017";
});
```

The connection string can also come from your `appsettings.json` or any other .NET configuration provider.

Inject `IDataStore` wherever you need persistence:

```csharp
public class MyService(IDataStore store)
{
    public Task<MyDoc> AddAsync(MyDoc doc) =>
        store.Add("mydb", "mydocs", doc);

    public Task<IEnumerable<MyDoc>> GetAllAsync() =>
        store.Get<MyDoc>("mydb", "mydocs");

    public Task<IEnumerable<MyDoc>> FindAsync(string name) =>
        store.Get<MyDoc>("mydb", "mydocs", d => d.Name == name);

    public Task RemoveAsync(string id) =>
        store.Delete<MyDoc>("mydb", "mydocs", d => d.Id == id);

    public Task<MyDoc?> UpdateAsync(string id, MyDoc updated) =>
        store.Update<MyDoc>("mydb", "mydocs", d => d.Id == id, updated);
}
```

## API

`IDataStore` exposes five operations:

| Method | Description |
|---|---|
| `Add<T>(database, collection, item)` | Inserts a document and returns it |
| `Get<T>(database, collection)` | Returns all documents in a collection |
| `Get<T>(database, collection, predicate)` | Returns documents matching a LINQ predicate |
| `Delete<T>(database, collection, predicate)` | Deletes documents matching a LINQ predicate |
| `Update<T>(database, collection, predicate, item)` | Replaces the first document matching a LINQ predicate and returns the updated document, or `null` if no match was found |

All methods are async.

## Distributed Tracing

MongoDB operations are automatically instrumented via `DiagnosticsActivityEventSubscriber`, which emits `Activity` events compatible with OpenTelemetry. No additional configuration is required.

## Build

```bash
dotnet build
```

To produce a NuGet package (also runs automatically on build due to `GeneratePackageOnBuild`):

```bash
dotnet pack -c Release
```

The `.nupkg` will be written to `bin/Release/`.

## Documenter Settings
`DocumenterSettings` is a static class that provides the ability to set default/fallback values and configure how some of the plugin works. 

All of the values that are required by the plugin have defaults as outlined below. These can be changed as required.

The `DocumenterSettings` class is static so properties that are updated will be set globally. 

```csharp
DocumenterSettings.FallbackCategory = "Fallback Category";
DocumenterSettings.DefaultTags = new[] { "DefaultTag" };
```

For ease of setting multiple values there is a `With()` method.
```csharp
DocumenterSettings.With(fallbackCategory: "Fallback Category", defaultTags: new[] { "DefaultTag" });
```

The `DocumenterSettings` class can also be scoped to certain blocks of code using the 
`With()` method. When this is disposed any previously set values will be returned.
```csharp
// Set fallback category globally
DocumenterSettings.FallbackCategory = "Global Category";

using (DocumenterSettings.With(fallbackCategory: "Scoped Category"))
{
    Console.WriteLine(DocumenterSettings.FallbackCategory); // output - "Scoped Category"
}
Console.WriteLine(DocumenterSettings.FallbackCategory); // output - "Global Category"

```

### Settings
The following is a list of the available properties, their use and any defaults.

| Name | Type | Use | Default |
| --- | --- | --- | --- |
| ReplacementVerbs | `IEnumerable<string>` | List of verbs that are output in documetation if `Any` supported by service. | GET, POST |
| Assemblies | `IEnumerable<Assembly>` | List of assemblies to scan for implementations of `AbstractTypeSpec<>` | `Assembly.GetEntryAssembly()` |
| CollectionStrategy | `EnrichmentStrategy` | How to deal with collection properties. Options are `Union` - each enricher will provide values and result will be unique union of these. `SetIfEmpty` - lower level enrichers will only provide values if the collection is null or empty. | `EnrichmentStrategy.Union` |
| FallbackNotes | `string` | Notes to fallback to if no other provided for resource | |
| FallbackCategory | `string` | Category to fallback to if no other provided | |
| FallbackRouteNotes | `string` | Notes to fallback to if no other provided for route | |
| DefaultStatusCodes| `IEnumerable<StatusCode>` | Status code to be set for all requests (e.g. 429 if RateLimiting is enabled). The usage of these will depend on value of CollectionStrategy. | |
| DefaultTags| `IEnumerable<string>` | Default tags to set for requests. The usage of these will depend on value of CollectionStrategy. | |
| DefaultContentTypes| `IEnumerable<string>` | Default content types (e.g. application/x-custom) to set for requests. The usage of these will depend on value of CollectionStrategy. | |
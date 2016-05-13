# ServiceStack.Request.Correlation

A plugin for [ServiceStack](https://servicestack.net/) that generates a series of POCOs documenting all available services as well as request and response DTOs. These POCOs will allow the data tbe be visualised in a number of standard API documentation formats (e.g. postman, swagger, RAML).

The plugin uses introspection on a number of different sources to generate as rich a set of documentation possible.

## Quick Start

The plugin is added like any other. It has a dependency [Metadata Plugin](https://github.com/ServiceStack/ServiceStack/wiki/Metadata-page) and by default requires that `AppHost.Config.WebHostUrl` is set. It also takes an instance of `ApiSpecConfig` as a constructor argument, as a minimum this must be populated with `Contact.Email`, `Contact.Name` and `Description`.
```csharp
public override void Configure(Container container)
{
    SetConfig(new HostConfig
    {
        // Required to know base URL
        WebHostUrl = "http://api.example.com:8001",
    });

	var apiSpecConfig = new ApiSpecConfig
    {
        Contact = new ApiContact { Email = "test@example.com", Name = "Joe Bloggs" },
        Description = "Service description"
    };

	// Register plugin
	Plugins.Add(new ApiSpecFeature(apiSpecConfig));
}
```

When the service starts up this will generate a list of all documentation. The plugin will also render a service which can be accessed at `/spec` to view the generated documentation.

## Documenting DTOs
The plugin uses a series of 'enrichers' to generate documentation, these are called in the order as outlined below. By default the approach is for each enricher to only set a value if it has not already been set. The exception for this is if the value is an array (e.g. an array of StatusCodes that may be returned) in this instance the strategy is to union results from various enrichers. This default can be controlled via the `DocumenterSettings` class.

### ReflectionEnricher
As the name suggests the `ReflectionEnricher` uses reflection to get details about DTOs. The majority of values are taken from attributes.

For example the following class would look at a combination of `[Api]`, `[ApiResponse]`, `[Route]`, `[ApiMember]`, `[IgnoreDataMember]` and `[ApiAllowableValues]` attributes to generate the documentation.

```csharp
[Api("Demo Request Description")]
[ApiResponse(HttpStatusCode.OK, "Everything is hunky dory")]
[ApiResponse(HttpStatusCode.InternalServerError, "Something went wrong")]
[Route("/request/{Name}", "GET,POST", Summary = "Route summary", Notes = "Notes about request")]
public class DemoRequest : IReturn<DemoResponse>
{
    [ApiMember(Name = "Name parameter", Description = "This is a description of name", ParameterType = "body", DataType = "string", IsRequired = true)]
    [ApiAllowableValues("Name", typeof(NameEnum))]
    public string Name { get; set; }

    [IgnoreDataMember]
    public string Ignored { get; set; }

    [ApiMember(ExcludeInSchema = true)]
    public int Optional { get; set; }
}

[Api("Demo Response Description")]
public class DemoResponse
{
	[ApiMember(Name="Response Message", Description = "The returned message")]
	public string Message { get; set; }
}
```
Although some of these attributes are originally for alternative purposes they give a good description of the DTO.

_TODO: Full details of where each property comes from_

### AbstractClassEnricher
This uses an approach similar to [FluentValidation](https://github.com/ServiceStack/ServiceStack/wiki/Validation#fluentvalidation-for-request-dtos). 

The enricher scans for implementations of `RequestDtoSpec<T>` (for RequestDTOs) `ApiDtoSpec<T>` (any other classes to be documented, e.g. embedded classes or Response DTOs) and generates documentation based on this. E.g.

```charp
public class DemoRequestDocumenter : RequestDtoSpec<DemoRequest>
{
    public DemoRequestDocumenter()
    {
        Title = "Plain request title from abstract";
        Description = "Demo Request Description";
        Notes = "Notes about request";

        Category = "Category1";

        AddVerbs("GET", "POST");
        AddTags("Tag1", "Tag2", "Tag3");

        AddStatusCodes(
            new StatusCode
            {
                Code = 500,
                Name = "Internal Server Error",
                Description = "Something went wrong"
            },
            (StatusCode)HttpStatusCode.OK);

        For(t => t.Name)
            .With(p => p.Title, "Name parameter")
            .With(p => p.IsRequired, true)
            .With(p => p.Description, "This is a description of name");

        For(t => t.Optional)
            .With(p => p.Title, "This is an optional thing.")
            .With(p => p.IsRequired, false);
    }
}

public class DemoResponseDocumenter : ApiDtoSpec<DemoResponse>
{
    public DemoResponseDocumenter()
    {
        Title = "Demo response title from documenter";
        Description = "Demo Response Description";
		
		For(t => t.Message)
			.With(p => p.Title, "Response Message")
            .With(p => p.Description, "The returned message");
    }
}
```
This approach allows for very explicit setting of properties as these classes are specifically for this purpose.
_TODO: Full details of where each property comes from_

### XmlEnricher
This uses the standard C# Xml Documentation Comments to generate documentation. 

For this to work the XML documentation file must be generated for the service. To do so RMC project -> Properties -> Build -> check "XML documentation file" box, _insert screenshot_.

```csharp

/// <summary>
/// 
/// </summary>
[Api("Demo Request Description")]
[ApiResponse(HttpStatusCode.OK, "Everything is hunky dory")]
[ApiResponse(HttpStatusCode.InternalServerError, "Something went wrong")]
[Route("/request/{Name}", "GET,POST", Summary = "Route summary", Notes = "Notes about request")]
public class DemoRequest : IReturn<DemoResponse>
{
    [ApiMember(Name = "Name parameter", Description = "This is a description of name", ParameterType = "body", DataType = "string", IsRequired = true)]
    [ApiAllowableValues("Name", typeof(NameEnum))]
    public string Name { get; set; }

    [IgnoreDataMember]
    public string Ignored { get; set; }

    [ApiMember(ExcludeInSchema = true)]
    public int Optional { get; set; }
}

[Api("Demo Response Description")]
public class DemoResponse
{
	[ApiMember(Name="Response Message", Description = "The returned message")]
	public string Message { get; set; }
}
```

The XML documentation comments are for general documentation about classes and not specifically for documentating APIs and DTOs but if need be these values can be used.

_TODO: Full details of where each property comes from_

## Customising
The plugin filters the `Metadata.OperationsMap` to get a list of `Operation` objects that contain the requests to be documented. This filter can be customised by providing a predicate to the plugin using the `ApiSpecFeature.WithOperationsFilter()` method. The default filter excludes any types that have `[Exclude(Feature.Metadata]` or `[Exclude(Feature.ServiceDiscovery]` or any restrictions.



## Settings
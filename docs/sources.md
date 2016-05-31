## Information Sources
The following tables document where the various reflections read information from.

By default the approach is for each enricher to only set a value if it has not already been set. The exception for this is if the value is an array (e.g. an array of StatusCodes that may be returned) in this instance the strategy is to union results from various enrichers. This default can be controlled via the `DocumenterSettings.CollectionStrategy` setting.

The fields that each provider populates differs as not every source can supply all data. There are 5 interfaces that the enrichers can implement:

* `IResourceEnricher` - a Resource is any request/response DTO type, or types embedded in a request/response DTO.
* `IPropertyEnricher` - a property is an individual property of a resource.
* `IRequestEnricher` - the IRequestEnricher is for populating fields related to Request DTOs.
* `IActionEnricher` - an Action is a Verb for a request (e.g. PersonRequest GET and PersonRequest POST are 2 different actions). This allows for handling of different routes or authentication settings per verb.
* `ISecurityEnricher` - for populating any security information (roles, permissions etc).

### Reflection Enricher
Reflection is the main source of information as it is what the underlying framework uses to process/restrict/construct requests.
| Field | Source | Notes |
| --- | --- | --- |
| Type.Title | Type.Name | |
| Type Description | `[Api]` -> `[ComponentModel].Description` -> `[DataAnnotations].Description` | |
| Security | `[Authenticate]` + `[RequiresAnyRole]` + `[RequiredRole]` + `[RequiresAnyPermission]` + `[RequiredPermission]` | Per verb |
| Content Types | `MetadataPagesConfig.AvailableFormatConfigs` + `[AddHeader].ContentType` -> `[AddHeader].DefaultContentType`. Filtered by `[Restrict]` + `[Exclude]` | Per verb |
| Relative Path | `[Route].Path` or OneWay / Reply URL | Per verb |
| Status Codes | `[ApiResponse]`. 401 + 403 added if requires authentication. 204 added if oneway. | Per Verb |
| Route Notes | `[Route].Notes` | Per Verb |
| Property Title | `[ApiMemberAttribute].Name` | |
| Property Description | `[ApiMemberAttribute].Description` | |
| Property Allow Multiple | `[ApiMemberAttribute].AllowMultiple` | |
| Property Constraints | `[ApiAllowableValues]` | |
| Property Is Required | `[ApiMemberAttribute].IsRequired` or true if type is `Nullable<>` | |
| Property Param Type | `[ApiMemberAttribute].ParameterType` | |

### Abstract Class Enricher
This comes from implementations of `TypeSpec<T>` or `RequestSpec<T>` so has properties explicitly set for each field.
| Field | Source | Notes |
| --- | --- | --- |
| Type.Title | `Title` | |
| Type Description | `Description` | |
| Type Notes | `Notes` | |
| Category | `Category` | |
| Tags | `Tags` | |
| Content Types | `ContentTypes` | Global or per verb |
| Status Codes | `StatusCodes` | Global or per verb |
| Route Notes | `RouteNotes` | Global or per verb |
| Property Title | `Title` | Uses `.For()` syntax to edit values for property|
| Property Description | `Description` | Uses `.For()` syntax to edit values for property |
| Property Constraints | `Constraints` | Uses `.For()` syntax to edit values for property |
| Property Is Required | `IsRequired` | Uses `.For()` syntax to edit values for property |
| Property Allow Multiple | `AllowMultiple` | Uses `.For()` syntax to edit values for property |

### XML Enricher
| Field | Source | Notes |
| --- | --- | --- |
| Type Description | `<summary>` | |
| Type Notes | `<remarks>` | |
| Property Description | `<summary>` | |
| Property Notes | `<remarks>` | |
| Status Codes | `<exception>` | Will apply across all verbs. Uses ServiceStack logic for working out StatusCode from Exception type. |

### Fallback Enricher
| Field | Source | Notes |
| --- | --- | --- |
| Type Notes | `DocumenterSettings.FallbackNotes` | These will be used in the event of no other notes being found. |
| Category | `DocumenterSettings.FallbackCategory` | This will be used in the event of no other category being found. |
| Tags | `DocumenterSettings.DefaultTags` | By default these will be combined with any other tags.|
| Content Types | `DocumenterSettings.DefaultContentTypes` | By default these will be combined with any other content types. |
| Status Codes | `DocumenterSettings.DefaultStatusCodes` | By default these will be combined with any other status codes. |
| Route Notes | `DocumenterSettings.FallbackRouteNotes` | This will be used in the event of no other notes being found. |
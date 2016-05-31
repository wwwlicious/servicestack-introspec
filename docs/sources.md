## Information Sources
The following tables document where the various reflections read information from.

### Reflection Enricher
Reflection is the main source of information as it is what the underlying framework uses to process/restrict/construct requests.
| Field | Source | Notes |
| --- | --- | --- |
| Type.Title | Type.Name | |
| Type Description | `[Api]` -> `[ComponentModel].Description` -> `[DataAnnotations].Description` | |
| Type Notes | `[RouteAttribute].Notes` | |
| Security | `[Authenticate]` + `[RequiresAnyRole]` + `[RequiredRole]` + `[RequiresAnyPermission]` + `[RequiredPermission]` | Per verb |
| Content Types | `MetadataPagesConfig.AvailableFormatConfigs` + `[AddHeader].ContentType` -> `[AddHeader].DefaultContentType`. Filtered by `[Restrict]` + `[Exclude]` | Per verb |
| Relative Path | `[Route]` or OneWay / Reply URL | Per verb |
| Status Codes | `[ApiResponse]`. 401 + 403 added if requires authentication. 204 added if oneway. | |
| Property Title | `[ApiMemberAttribute].Name` | |
| Property Description | `[ApiMemberAttribute].Description` | |
| Property Allow Multiple | `[ApiMemberAttribute].AllowMultiple` | |
| Property Constraints | `[ApiAllowableValues]` | |
| Property Is Required | `[ApiMemberAttribute].IsRequired` | |
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

### Fallback Enricher
| Field | Source | Notes |
| --- | --- | --- |
| Type Notes | `DocumenterSettings.FallbackNotes` | These will be used in the event of no other notes being found. |
| Category | `DocumenterSettings.FallbackCategory` | This will be used in the event of no other category being found. |
| Tags | `DocumenterSettings.DefaultTags` | By default these will be combined with any other tags.|
| Content Types | `DocumenterSettings.DefaultContentTypes` | By default these will be combined with any other content types. |
| Status Codes | `DocumenterSettings.DefaultStatusCodes` | By default these will be combined with any other status codes. |
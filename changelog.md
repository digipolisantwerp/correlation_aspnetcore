# Correlation Toolbox

##5.0.0

- .NET Core 2.1 version.
- Added AddCorrelationHeaderHandler, a DelegatingHandler, to add the correlation header to the HttpRequest's Headers.
- Use of the HttpContext to pass the correlation header to the HttpClient/DelegatingHandler.
- Switched to manual serialization of the correlation context to json with JsonTextWriter for performance.
- Changed the HttpClientExtensions to use the header string on the correlation context in stead of creating the string ad hoc.

## 4.1.1

- Include Dgp Header in interface

## 4.0.0

- Added CorrelationMiddleware that uses the correlationheader to fill the correlationcontext
- Updated the model and headerkeys used for the correlationheader
- Use of IApplicationContext to create a new correlationContext

## 3.0.0

- conversion to csproj and MSBuild.

## 2.0.0

- .NET Core 1.0 version

## 1.0.0

- initial version

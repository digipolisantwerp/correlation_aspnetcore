# Correlation Toolbox

Toolbox for correlation id's.

## Table of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->


- [Installation](#installation)
- [Usage](#usage)
- [HttpClientExtensions](#httpclientextensions)
- [Coupling with a Service Agent](#coupling-with-a-service-agent)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Installation

To add the toolbox to a project, you add the package to the csproj project file:

```xml
  <ItemGroup>
    <PackageReference Include="Digipolis.Correlation" Version="4.1.0" />
  </ItemGroup>
``` 

or if your project still works with project.json :

``` json 
"dependencies": {
    "Digipolis.Correlation":  "4.1.0"
 }
``` 

In Visual Studio you can also use the NuGet Package Manager to do this.

## Usage

The correlation header contains an identifier in the form of a guid that can be set on an http request by use of headers. The purpose is to track different requests that are related to each other when a request causes a chain of api calls.
Together with the id a sourceId, sourceName, instanceName, instanceId, userId and ipAddress property is available and can also be set.

When the Correlation middelware is used an **ICorrelationContext** object becomes available and can be injected into a class by the dependency injection framework.
When the correlation context is requested in classes that are created after the CorrelationId middelware has executed, the context will contain all values needed to use in subsequent api calls.

If the incoming request contains the correlation headers, those values are set on the context object.
If the incoming request does not contain the correlation headers a new correlation id will be created and the other properties will be set using the **IApplicationContext**..

To use the correlationId middelware two steps are needed.

First register the service in the **ConfigureServices** method in the **Startup** class:

With the default options:
``` csharp
  services.AddCorrelation();
```

With custom options:
``` csharp
  service.AddCorrelation(options => 
  {
     options.CorrelationHeaderRequired = true
  });
```

Following options can be set :

Option              | Description                                                | Default
------------------ | ----------------------------------------------------------- | --------------------------------------
CorrelationHeaderRequired              | If set to true a Digipolis.Errors.ValidationException is thrown when the Dgp-Correlation header is missing | false
CorrelationHeaderNotRequiredRouteRegex  | Routes matching this regex will never require the correlationheader. By default /vx/status and /status will never require the correlation header. | ^(/v./(?i)(status)/\|/(?i)(status)/)

Then add the middleware to the appication in the **Configure** method in the **Startup** class:

``` csharp
  app.UseCorrelation();
```

Please note that the order in wich middleware is added is the order of execution of the middleware. Putting UseCorrelation() (with correlationheader required) before UseSwaggerUI() will make the SwaggerUI fail. UseCorrelation should come after UseSwaggerUI() and before UseMvc().

## HttpClientExtensions

In order to use the correlation values when calling another api you can use the **HttpClient** extension methods provided in this Digipolis.

Two overloads are available. The first takes an **ICorrelationContext** as argument and sets the values on the headers.
``` csharp
  client.SetCorrelationValues(context);
```

The second takes an **IServiceProvider** as argument where it can request for the correlation context and then set the values on the request headers.
``` csharp
  client.SetCorrelationValues(serviceProvider);
```

Depending on what instance you already have available you can choose one over the other.

The extension methods will set the headers on the http client. You only need tot do this once when using the client to make multiple call's to the api.

Important notice is that it is assumed that the http client is a transient or scoped instance, not a singleton!

## Coupling with a Service Agent

When using the correlation in conjunction with a service agent (form Digipolis.ServiceAgent) it is possible setup the coupling in the **ConfigureServices** method of the **Startup** class.
You can supply an action that gets invoked by the service agent toolbox when the service agent gets instantiated. On that action you can use the extension methods to setup the correlation usage.
``` csharp
    services.AddSingleServiceAgent<SampleApi2Agent>(settings =>
    {
        settings.Scheme = HttpSchema.Http;
        settings.Host = "localhost";
        settings.Port = "5001";
        settings.Path = "api/";
    }, (serviceProvider, client) => client.SetCorrelationValues(serviceProvider));
```

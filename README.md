# Correlation Toolbox

Toolbox for correlation id's.

## Table of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->

- [Installation](#installation)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Installation

To add the toolbox to a project, you add the package to the project.json :

``` json 
"dependencies": {
    "Toolbox.Correlation":  "1.0.0"
 }
``` 

In Visual Studio you can also use the NuGet Package Manager to do this.

## Usage

The correlation id is an identifier in the form of a guid that can be set on an http request by use of headers. The purpose is to track different requests that are related to each other when a request causes a chain of api calls.
Together with the id a source property is available and can also be set.

When the Correlation middelware is used an **ICorrelationContext** object can be injected into a class by the dependency injection framework.
When the correlation context is requested in classes that are created after the CorrelationId middelware has executed, the context will contain all values needed to use in subsequent api calls.

If the incomming request contains the correlation headers, those values are set on the context object.
If the incomming request does not contain the correlation headers a new correlation id will be created and the source will be set to the value passed in the **UseCorrelationId** method in the **Startup** class.

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
     options.IdHeaderKey = "CustomIdHeaderKey",
     options.SourceHeaderKey = "CustomSourceHeaderKey", 
      
  });
```

Following options can be set :

Option              | Description                                                | Default
------------------ | ----------------------------------------------------------- | --------------------------------------
IdHeaderKey              | The header key used for the correlation id value. | "D-Correlation-Id"
SourceHeaderKey | The header key used for the correlation source value. | "D-Correlation-Source"  

Then add the middleware to the appication in the **Configure** method in the **Startup** class:

``` csharp
  app.UseCorrelation("SoureValue");
```
The argument in the **UseCorreltationId** method sets the value to be used as the correlation source in the case the correlation id is generated in the application.

Please note that the order in wich middleware is added is the order of execution of the middleware. Thus middleware in the pipeline previous to the correlationId middleware will not be able to use the correlationId values.
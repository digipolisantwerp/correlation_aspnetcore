# Correlation Toolbox

## 6.0.1

- Downgpgrade to net standard 2.0 for backwards compatibility

## 6.0.0

- Upgrade to net standard 2.1

## 5.3.1

- Bugfix: invalid correlation headers will be overwritten by a default header before logging and throwing an exception to stop an infinite loop. Now catching even more exceptions


## 5.3.0

- Invalid correlation headers will be overwritten by a default header before logging and throwing an exception to stop an infinite loop 

## 5.2.1

- Fix scoped correlation context

## 5.2.0

- Generate dgpheader if not present and header not required

## 5.1.0

- Fixed broken correlationidhandler

## 5.0.2

- Casing bugfix

## 5.0.0

- Upgrade to net standard 2.0
- Usage of a service to get/create the correlationcontext

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

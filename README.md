
# A Testable Entity Framework Abstraction

The code contained in this repository reflects many hours of work coming to a ideal situation in that redundant functionality is abstracted out into reusable classes.

**For instructions on testing/mocking data sets and contexts, [see the Wiki](https://github.com/MobileApped-Public/MobileApped.Core.Persistence/wiki)**

# Package Installation
Multiple packages exist in NuGet.org 
* [MobileApped.Core.Persistence](http://www.nuget.org/packages/MobileApped.Core.Persistence/)
* [MobileApped.Core.Persistence.InMemory](http://www.nuget.org/packages/MobileApped.Core.Persistence.InMemory/)
* [MobileApped.Core.Persistence.SqlServer](http://www.nuget.org/packages/MobileApped.Core.Persistence.SqlServer/)
* [MobileApped.Core.Persistence.Postgres](http://www.nuget.org/packages/MobileApped.Core.Persistence.Postgres/)


Install using the Visual Studio package manager or
from the package manager console run the following:

For SqlServer:
```
PM> Install-Package MobileApped.Core.Persistence.SqlServer
```
For Postgres:
```
PM> Install-Package MobileApped.Core.Persistence.Postgres
```
For Testing:
```
PM> Install-Package MobileApped.Core.Persistence.InMemory
```

# Example of Basic Usage
### Database Connection Strings 
In the application's conifguration file (app.config or web.config), verify that eixsts or add the connection string section.
```
"ConnectionStrings": {
    "[DatabaseName]": server=[host];database=[db name];
}
```
* Note: The Name of the [DatabaseName] tag needs to be the same that is used in the DbContext initializer.


### Register the new Context with the default IoC container
```
#!c#

services.
```


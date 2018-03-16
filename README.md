
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
# Getting started

### Create a AppSettings.json
``` json
{
    "ConnectionStrings": {
        "TestConnection" : "database connection string"
    }
}
```

### Create an Entity class
``` csharp
public class Person {
    [Key] public int ID { get; set; }
}
```

### Create a DbContext
``` csharp
public class PersonContext : DbContext {
    public PersonContext(DbContextOptions options) : base(options){} // This constructor is required when using this library

    public DbSet<Person> People { get; set; }
}
```


### Register a DbContext with the ServiceCollection
``` csharp
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("AppSettings.json")
    .Build();
IServiceCollection services = new ServiceCollection();
services.AddSqlServerDataContext<PersonContext>(configuration, "TestConnection");
```

### Consume the DataContext
``` csharp
public class HomeController : Controller {
    private IDataContext<PersonContext> context;
    public HomeController(IDataContext<PersonContext> context)
    { 
        this.context = context;
    }

    [HttpGet("api/people")]
    public IActionResult GetPeople()
    {
        var people = context.UsingContext(d => d.People.ToList());
        return Json(people);
    }
}
```


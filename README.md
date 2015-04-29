
# A Testable Entity Framework Abstraction

The code contained in this repository reflects many hours of work coming to a ideal situation in that redundant functionality is abstracted out into reusable classes.

**For instructions on testing/mocking data sets and contexts, [see the Wiki](https://github.com/MobileApped-Public/MobileApped.Core.Persistence/wiki)**

# Package Installation
This package is currently hosted on [Nuget.org](http://www.nuget.org/packages/MobileApped.Core.Persistence/)

Install using the Visual Studio package manager or
from the package manager console run:
```
PM> Install-Package MobileApped.Core.Persistence
```

# Example of Basic Usage
### Database Connection Strings 
In the application's conifguration file (app.config or web.config), verify that eixsts or add the connection string section.
```
<connectionStrings>
  <add name="[DatabaseName]Entities" connectionString="data source=[HOST];initial catalog=[DatabaseName];user id=[UserName];password=[Password];MultipleActiveResultSets=True;App=[ApplicationName];" providerName="System.Data.SqlClient" />
</connectionStrings>
```
* Note: The Name of the [DatabaseName] tag needs to be the same that is used in the DbContext constructor.

#### Octopus Deploy Template
Example Octopus Deploy template:

```
<connectionStrings>
  #{each connection in CONNECTIONS}
    <add name="#{connection.Name}" connectionString="#{connection.SourceAndUser};password=#{connection.Password};MultipleActiveResultSets=True;App=ApplicationName;" providerName="System.Data.SqlClient" />
  #{/each}
</connectionStrings>
```

## Adding Code
Add the following files/classes to your project:
* NorthwindContext.cs
* EmployeeEntity.cs
* EmployeeProvider.cs

#### NorthwindContext.cs - Typed DbContext
```
    [ExcludeFromCodeCoverage]
    public class NorthwindContext : DbContext
    {
        public NorthwindContext()
            : base("name=NorthwindEntities")
        {
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }
    
        public virtual DbSet<EmployeeEntity> Employees { get; set; }
    }
```

#### EmployeeEntity.cs - Entity Mapping to the Employee table
```
#!c#
    [Table("Employee")]
    [ExcludeFromCodeCoverage]
    public partial class EmployeeEntity : Entity
    {
         [Key]
         public int ID { get; set; }
    }
```

#### EmployeeProvider.cs - Table provider
```
private IDataContext<NorthwindContext> northwindContext;

// Create a context without injection
public EmployeeProvider()
{
    northwindContext = new DataContext<NorthwindContext>();
}

// Construct an instance with an injected context
public EmployeeProvider(IDataContext<NorthwindContext> northwindContext)
{
    this.northwindContext = northwindContext;
}

public EmployeeEntity Get(int id)
{
    return northwindContext.SingleOrDefault<EmployeeEntity>(d => d.ID == id);
}
```

### Register the new Context with your prefered IoC Container
* Note: The preferred lifetime is a Singleton
```
#!c#
public void RegisterDepedencies()
{
     container.Register<IDataContext<NorthwindContext>, DataContext<NorthwindContext>>();
}
```


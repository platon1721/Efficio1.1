# Demo

~~~sh
dotnet tool update -g dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator

dotnet ef migrations add --project App.DAL.EF --startup-project WebApp --context AppDbContext InitialCreate

dotnet ef migrations   --project App.DAL.EF --startup-project WebApp remove

dotnet ef database   --project App.DAL.EF --startup-project WebApp update
dotnet ef database   --project App.DAL.EF --startup-project WebApp drop

~~~

MVC controllers
~~~sh
cd WebApp

dotnet aspnet-codegenerator controller -name PersonsController        -actions -m  Domain.Person        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name ContactTypesController        -actions -m  Domain.ContactType        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name ContactsController        -actions -m  Domain.Contact        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name PersonsController        -actions -m  App.Domain.Person        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name ContactTypesController        -actions -m  App.Domain.ContactType        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name ContactsController        -actions -m  App.Domain.Contact        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -name RefreshTokensController        -actions -m  App.Domain.Identity.AppRefreshToken        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name UsersController        -actions -m  App.Domain.Identity.AppUser        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name RolesController        -actions -m  App.Domain.Identity.AppRole        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name UserRolesController        -actions -m  App.Domain.Identity.AppUserRole        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f


dotnet aspnet-codegenerator identity -dc App.DAL.EF.AppDbContext -f
~~~


Api controllers
~~~bash
dotnet aspnet-codegenerator controller -name PersonsController  -m  Domain.Person        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name ContactTypesController  -m  Domain.ContactType        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
dotnet aspnet-codegenerator controller -name ContactsController  -m  Domain.Contact        -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f
~~~

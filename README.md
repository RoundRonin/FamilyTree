# FamilyTreeBlazor

## Initialization of the project

WARNING! Env file is shipped with the project as .env.sample (copy and remove .sample to use)

.env should look like this:
```
POSTGRES_USER=mahUsername
POSTGRES_PASSWORD=mahPassword
POSTGRES_DB=FamilyTreeDB
HOST=localhost
PORT=5432
```

### Databse

From the root of the project

``` Sh
cd DAL
dotnet ef migrations add InitialCreate --startup-project ..\Presentation\
dotnet ef database update --startup-project ..\Presentation\
```
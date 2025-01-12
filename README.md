# FamilyTree

FamilyTree is a web application that allows user to create and manage famaly tree. App's main features include advanced UI, data persistance between sessions, several features to view and manage tree data.

## Description

### Features 
Interactivity:
1. Tree view -- gives a visual representation of the relationships in a family. Also allowes to get details on a specific person
2. Common ancestors -- finds common ancestors for any two members of a family
3. Ancestor age -- calculates age of an ancestor when their descendant was born
4. Editing instrument -- allows to make changes to a tree: add a new person by relationship to an existing person, add new relationships between existing members of a family
5. Delete tree -- allows to delete the entire tree from the database
6. Toggle dragging -- allows tree rearrangement. No persistance is provided yet

Internal features:
1. Keeps tree data persistant between sessions

### Developer info 

App uses 
1. BLazor UI with interactive SSR on the GUI side 
    1. MudBlazor for components
    2. D3.js for the graph
2. PostgreSQL in a docker container on the repository side. 

App utilizes several architecture and design patterns. From a developer standpoint it is impoortant to note that the app works with a graph of a family, not with a tree (graph is a superset of a tree), because unlike a tree it allows to represent realistic relationships between family members.

#### Presentation -- BLL -- DAL structure
App uses clear devision in three layer with each level managing it's own responsibilities

DAL manages it's own repository interactions. Currently PostgreSQL interaction is implemented. Provides entities to interact with from within this layer and from BLL.

BLL manages business-related funcitonality of the app. It provides an actual tree representation using composite pattern and all the methods of this layer utilize this pattern to interact with data. Also, consistensy between BLL and DAL is provided on this layer -- each operation that changes the data always ensures consistensy between composite tree and permanent srorage (currently, tables in PostgreSQL database).

Presentation layer manages graph's UI representation. Blazor UI with interactive server side rendering (SSR) is used. Graph view is implemented using D3.js library (using it's physical graph API), so JavaScript is used to create an API for the app's needs invokable from C#. This layer implements state pattern to manage UI states and command pattern to pass specific command to the state. Services on this layer are created purely to deal with the needs of UI.

#### Patterns

1. Composite in BLL to represent the graph
2. State in Presentation layer to manage UI state 
3. Command in Presnetation layer to send complex commands to the state

Besides that some inherit to the main app sturctures patterns and techniques are present such as dependency injection, repository, events, etc.

### Testing

Currently, there is a test project that was created for an older version of the App (where it had covered the entire BLL and DAL with around 30 tests), but it is hasn't been updated and is outdated as of now.

### NuGet Packages Used

- Microsoft.EntityFrameworkCore: Core EF package.
- Microsoft.EntityFrameworkCore.Design: Design-time tools for EF.
- Microsoft.EntityFrameworkCore.Tools: Command-line tools for EF.
- DotNetEnv

And some other packages for testing

- xUnit
- moq
- EntityFrameworkCore.InMemory

## Initialization of the project

### Requirements:

1. Docker
2. Dotnet
3. Dotnet-ef

1. Git
2. IDE

WARNING! Env file is shipped with the project as .env.sample (copy and remove .sample to use)

.env should look like this:
```
POSTGRES_USER=mahUsername
POSTGRES_PASSWORD=mahPassword
POSTGRES_DB=FamilyTreeDB
HOST=localhost
PORT=5432
```

To install dotnet-ef on linux (needed to run migrations):
```sh
dotnet tool install --global dotnet-ef --version 9.*
```

### Start-up, all the steps

```sh
docker compose up
```

after the DB is running inside docker container run

First, launch the container.

To start:
``` Sh
docker-compose up -d
```

To stop:
``` Sh
docker-compose down
```

From the root of the project run migrations

To restore all the nuget packages
```sh
dotnet restore
```

To create and apply migrations:
```sh
cd DAL
dotnet ef migrations add InitialCreate --startup-project ../Presentation/
dotnet ef database update --startup-project ../Presentation/
```

```sh
cd ..
dotnet run --project ./Presentation/FamilyTreeBlazor.presentation.csproj
```

Or 

```sh
cd .. 
dotnet watch --project ./Presentation/FamilyTreeBlazor.presentation.csproj
```

# Contribution

Feel free to contribute!

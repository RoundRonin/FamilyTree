# FamilyTree

The app is build with extensive testing -- more than 35 tests are covering all the entirity of BLL interfaces.

## Project Structure

The project is organized into several layers:

1. Presentation: Contains the logic to display UI using blazor
2. Business Logic Layer (BLL): Contains the services responsible for the business logic. DTOs -- are the models here.
3. Data Access Layer (DAL): Contains the repositories responsible for data access. Entities -- are the models here. They contain the data models representing the database entities.

## Key Patterns

1. Repository Pattern: Abstracts the data layer, providing a clear separation between data access and business logic.
2. Compsite: Used to create cache that stores the entire tree during one session.
3. Dependency Injection: Ensures loose coupling and enhances testability. Provided by Blazor.
4. Data Mapper is used for the database, provided by Microsoft with Entity Framework.
5. Anemic domain model is used to manage business logic. DTOs represent the real-life structure and services provide the functionality.
6. Async/Await: Utilized throughout to ensure non-blocking operations.

## Usage

### Prerequisites 
- .NET SDK installed 
- Docker for PostgreSQL database (if using the database repository)

Clone the repository:
``` Sh
git clone https://github.com/RoundRonin/FamilyTree.git
cd FamilyTree
```
Install Dependencies:
``` Sh
dotnet restore
```

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

``` Sh
cd DAL
dotnet ef migrations add InitialCreate --startup-project ..\Presentation\
dotnet ef database update --startup-project ..\Presentation\
```

### Run the Application
``` Sh
dotnet run
```
Apps is available at:
http://localhost:5000/

## NuGet Packages Used

- Microsoft.EntityFrameworkCore: Core EF package.
- Microsoft.EntityFrameworkCore.Design: Design-time tools for EF.
- Microsoft.EntityFrameworkCore.Tools: Command-line tools for EF.
- DotNetEnv

And some other packages for testing

- xUnit
- moq
- EntityFrameworkCore.InMemory

## Development Environment

This project was developed using Visual Studio, git.

## Contribution

Feel free to not contirbute at all!

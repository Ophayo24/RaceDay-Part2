# RaceDay — Event Management System (Part 2)

    Full-stack event management platform for the South African road running, walking, and cycling community.

    ## Roles
    - **Organiser** — create and manage events, categories, enrolments, and results.
    - **Participant** — browse events, enter a category, track personal results.

    ## Tech Stack
    - .NET 8 Web API
    - Entity Framework Core 8 (Code-First)
    - SQL Server / LocalDB
    - BCrypt password hashing
    - Swagger 
    - xUnit + EF Core InMemory
    - GitHub Actions CI

    ## Setup
    1. Clone repo.
    2. Open `RaceDay.sln` in Visual Studio 2022.
    3. Restore NuGet packages.
    4. Ensure LocalDB is running, or update `appsettings.json` connection string.
    5. Package Manager Console:
       - `Add-Migration InitialCreate` (if Migrations folder is empty)
       - `Update-Database`
    6. Press F5. Swagger opens at `/swagger`.

    ## Endpoints
    See `/docs/API Endpoint Plan.pdf` (Part 1) — this API implements that plan.


    ## Video
    [YouTube walkthrough] https://youtu.be/wNcqx7GT7WI

    ## AI Disclosure
    AI assistance was used to explain EF Core patterns and structure unit tests.

    Replace `ci-screenshot.png` with your actual screenshot inside `/docs`.
    Replace the YouTube link after recording.

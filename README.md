# RaceDay-System-Part-2
PROG6212 P2
Youtube Video Link: 

RaceDay System – Part 2

1. Project Overview

RaceDay System is an ASP.NET Core Web API developed to manage race events and participant information. Part 2 focuses on developing API endpoints, connecting to the database, implementing authentication and role-based access, testing functionality, and managing the project using GitHub.

2. Technologies Used

* C# and .NET 8 – API development.
* ASP.NET Core Web API – Handles HTTP requests and responses.
* SQL Server – Stores application data.
* Entity Framework Core – Supports database access.
* Swagger – Allows API endpoints to be viewed and tested.
* Visual Studio 2022 – Development environment.
* Git and GitHub – Source control and project collaboration.
* xUnit – Unit testing.
* GitHub Actions – Automated build and test workflow.

3. Main Features

User Management and Authentication

The system supports user information retrieval, registration and login functionality. Passwords are handled using password hashing, and session-based authentication is used to identify logged-in users.

Role-Based Access Control

The system distinguishes between two main roles:

* Organizer: Manages race events, categories and race results.
* Participant: Views available events, enrolls in events and accesses their own results.

Protected endpoints restrict access according to the user’s role and permissions.

Event Management

The Events API handles race event information, including event names, descriptions, dates, locations, distances and event types.

Category Management

The Categories API manages race categories and related information, such as age limits and race distances.

Enrollments and Results

The Enrollments API manages participant enrollment records. The Results API provides access to race results while applying role-based access restrictions.

Data Transfer Objects (DTOs)

DTOs structure the information accepted and returned by the API. They help control which information is exposed to clients and keep sensitive information, such as password hashes, out of user response data.

4. API Controllers

The project includes controllers for:

* Users
* Authentication
* Events
* Categories
* Enrollments
* Results
* Roles

These controllers organise the API functionality into separate sections.

5. Database Integration

The API connects to a SQL Server database containing the application’s information. Database access is configured through the application, and the controllers use the stored data to respond to API requests.

6. API Testing

Swagger was used to inspect and test API endpoints. Requests were tested to check responses and identify errors.

The project also includes an xUnit test project, RaceDayAPI.Tests, with tests for event request validation. These tests check that valid event information passes validation and that an invalid race distance fails validation.

7. GitHub and Version Control

The project was uploaded to GitHub using Git. Separate commits were created to track project changes and document parts of the code through comments.

The repository contains the API project, test project and solution files.

Repository: https://github.com/NothandoMokoka/RaceDay-System-Part-2

8. GitHub Actions

A GitHub Actions workflow named RaceDay Build and Test was added to automate the process of restoring dependencies, building the solution and running unit tests.
<img width="1883" height="692" alt="image" src="https://github.com/user-attachments/assets/ccb8a0d4-0c52-4071-97ab-0d2247d34899" />

The initial workflow run failed because the workflow could not locate RaceDayAPI.sln at the repository root. The solution file path must be corrected before the workflow can complete successfully.

9. Conclusion

RaceDay System Part 2 develops the foundation of a race management API with database integration, authentication, role-based access, event and participant data management, and automated testing. GitHub version control and GitHub Actions are used to support source tracking and continuous integration.

Further verification is required to confirm that all endpoints work correctly, the automated workflow passes, and the final project meets all assessment requirements.

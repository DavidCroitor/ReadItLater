# ReadItLater

A web application for saving and managing articles to read later.

## Project Structure

The project follows a clean architecture pattern with the following components:

- **Backend**
  - API: Web API endpoints and controllers
  - Domain: Core business logic and entities
  - Data: Database access and repositories

## Technologies Used

- .NET 9.0
- Entity Framework Core 9.0
- PostgreSQL 9.0

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL database

### Setup

1. Clone the repository
   ```
   git clone https://github.com/DavidCroitor/ReadItLater.git
   cd ReadItLater
   ```

2. Update database connection string in appsettings.json

3. Run database migrations
   ```
   cd backend/API
   dotnet ef database update
   ```

4. Start the application
   ```
   dotnet run
   ```

## Features

- Save articles for later reading
- Organize articles by categories
- Mark articles as read/unread


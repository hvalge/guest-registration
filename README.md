# Guest Registration

This is a guest registration system that allows for the creation and management of future and past events. For each event, it's possible to add, view, edit, and delete participants, who can be either natural persons or legal entities.

## Architecture

The application is built using a layered architecture based on Domain-Driven Design principles, ensuring a separation of concerns and maintainability.

* **Core:** This layer contains the domain models and business logic of the application. It includes entities like `Event`, `Participant`, `NaturalPerson`, `LegalPerson`, and `PaymentMethod`, as well as interfaces for repositories. This layer has no dependencies on other layers in the solution.
* **Application:** This layer orchestrates the application's use cases by mediating between the presentation layer and the domain layer. It contains application services, DTOs (Data Transfer Objects), and exceptions.
* **Infrastructure:** This layer handles technical concerns like data persistence and external services. It includes implementations of the repository interfaces defined in the Core layer, using Entity Framework Core for database interaction.
* **API:** This is the presentation layer of the backend, implemented as an ASP.NET Core Web API. It exposes endpoints for the frontend to consume and handles HTTP requests and responses.

-----

## Technologies Used

### Backend

* **.NET 9**
* **ASP.NET Core:** For building the REST API.
* **Entity Framework Core:** As the Object-Relational Mapper (ORM) for database interactions.
* **SQLite:** As the database provider.
* **Serilog:** For logging.
* **xUnit & Moq:** For unit and integration testing.

### Frontend

* **React**
* **TypeScript**
* **Vite:** As the build tool and development server.
* **React Query:** For data fetching and state management.
* **Axios:** For making HTTP requests to the backend.
* **Bootstrap:** For styling the user interface.
* **React Router:** For client-side routing.

-----

## Installation and Setup

### Prerequisites

* .NET 9 SDK
* Node.js and npm

### Backend

1. **Clone the repository.**
2. **Navigate to the `backend/GuestRegistration.Api` directory.**
3. **Restore the .NET dependencies:**

    ```bash
    dotnet restore
    ```

4. **Run the database migrations to create the database:**

    ```bash
    dotnet ef database update
    ```

### Frontend

1. **Navigate to the `frontend` directory.**
2. **Install the npm dependencies:**

    ```bash
    npm install
    ```

-----

## Running the Application

### Backend

From the `backend/GuestRegistration.Api` directory, run:

```bash
dotnet run
```

The API will be available at `http://localhost:5188`.

### Frontend

From the `frontend` directory, run:

```bash
npm run dev
```

The frontend development server will start, and the application will be accessible at `http://localhost:5173`.

-----

## Testing

To run the automated tests for the backend, navigate to the `backend` directory and run:

```bash
dotnet test
```

This will execute both unit and integration tests.

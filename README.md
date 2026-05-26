# Payroll management

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![Version](https://img.shields.io/badge/version-1.0.0-blue)

---

## Description

The project consists of the development of a REST API based on an MVC (Model–View–Controller) architecture, focused on the secure and efficient management of user authentication, employee administration, and financial transactions that allow crediting or debiting employee balances.

The solution was designed following a separation of concerns approach, providing a maintainable, scalable, and well-structured architecture. All validation, processing, and data transmission logic is handled through a SQL Server database, using exclusively Stored Procedures for communication between the application and the database. As part of this approach, no hardcoded SQL queries were implemented within the web application, enhancing security, transactional control, and business logic reusability.

Additionally, the system includes mechanisms for initial data loading and information updates through XML files, which are used to populate catalog tables and dynamic system tables. This information is processed through SQL scripts responsible for mapping, transforming, and inserting the data in a structured and controlled manner.

The entire architecture and system design were developed with the ACID principles (Atomicity, Consistency, Isolation, and Durability) in mind, ensuring the integrity, reliability, and consistency of all critical transactions and operations performed by the system.

**Tech stack:**
- Language / Framework: `C# / ASP.NET Core 8`
- Database: `SQL Server`
- Packages: `Microsoft.EntityFrameworkCore.SqlServer (8.0.8), Microsoft.EntityFrameworkCore.Tools (8.0.8), Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.5), DotNetEnv (3.2.0)`

---

## Screenshots

The important views:

#### List of employes
![App Screenshot](/imgs/Empleados.png)

#### Update employe
![App Screenshot](/imgs/ActualizarEmpleado.png)

#### Employee movements
![App Screenshot](/imgs/MovimientosPorUsuario.png)

#### Add movements
![App Screenshot](/imgs/AgregarMovimiento.png)

---

## Project Structure

```
📦 Project
 ┣ 📂 .github/workflows           # CI/CD of GitHub Actions
 ┣ 📂 imgs                        # Imgs used in this file
 ┣ 📂 Solucion_Tarea2_BD1         # Source code
 ┃ ┗ 📂 Tarea2_BD1
 ┃   ┣ 📂 Properties              # Launch settings
 ┃   ┣ 📂 wwwroot                 # Styles, javascript, lib
 ┃   ┣ 📂 Controllers             # API route handlers
 ┃   ┣ 📂 Models                  # Data models and entities
 ┃   ┣ 📂 Views                   # Webpage views
 ┃   ┣ 📄 appsettings.json        # App configuration (no secrets)
 ┃   ┗ ▶️ Program.cs              # Launch settings
 ┣ 📂 SQL Database                # SQL Server Scripts
 ┣ 🚫 .gitignore                  # Files ignore
 ┗ 📄 README.md                   # This file
```

---

## Installation

Through the Visual Studio editor, you can download the frameworks to work with.

### Prerequisites

Make sure you have the following installed:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Git](https://git-scm.com/)

## Configuration

1. Create the database locally or in a some cloud service.

2. Use the .sql files in the carpet ```SQL Database``` to create all the tables and relations between tables.

3. Configure your ```.env``` with the string connection with a variable called ```ConnectionStrings__conexion=```

> **Note:** Never commit `.env`. It is already listed in `.gitignore`.

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/JoseJimenez01/Tarea2_BD1_2S_2024.git
   ```

2. Navigate to the project folder where the .csproj is.

3. Restore dependencies:
   ```bash
   dotnet restore
   ```

4. Build the project:
   ```bash
   dotnet build
   ```

---

## Running the Project

### Development

```bash
dotnet run
```

The app will be available at `https://localhost:<portNumber>`.

### Running Tests

```bash
dotnet test
```

---

## API Reference

| Method | Endpoint | Description | Auth required |
|--------|----------|-------------|---------------|
| POST | `/api/ConsultaInicioSesionFallido` | See how many times someone has tried to Login | No |
| POST | `/api/ConsultaCodError` | List all employees | No |
| POST | `/api/InicioDeSesion` | To login in the platform | No |
| POST | `/api/ValidarDataAnnotations` | Manage retroalimentation when login | No |
| GET | `/api/listar_empleados` | List all employees | No |
| POST | `/api/Filtrar` | Search for emplyees that matches some string | No |
| POST | `/api/agregarEmpleado` | Add a new employe | No |
| POST | `/api/actualizarEmpleado` | Update an employe | No |
| POST | `/api/ControlErroresActualizar` | Manage retroalimentation when updating an employe | No |
| POST | `/api/borrar-empleado` | Delete an employe | No |
| POST | `/api/ControlErroresBorrar` | Manage retroalimentation when deleting an employe | No |
| GET | `/api/listar_movimientos` | List all movements of an employe | No |
| GET | `/api/Empleado` | List all movements of an employe | No |
| POST | `/api/agregarMovimiento` | Add a movement to an employe | No |
| POST | `/api/ControlErrores` | Manage retroalimentation when adding a movement | No |

---

## Credentials

- User: UsuarioScripts
- Password: UsuarioScripts

---

## Future improves
>NOTE: this is a mid-career project, so reviewing it at the end, i can identify some things that i would change:

- Configure cookies for the autentications.
- Add path protection.
- Change how to query the DB, the amount of times someone has tried to login, don't using the logs, instead of that, using the table users with an attribute for attempts and another for time blocked, using triggers for this attributes.
- Use functions for instead of repetitive code, like the creation of store procedures in the controllers.
- Testing simulations.

---
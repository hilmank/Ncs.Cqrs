# NCS Cqrs API 🏪🍽️  
**CQRS-based REST API using .NET 8, Dapper, and MediatR**  

## 🔥 Overview  
NCS Cqrs API is a high-performance **REST API** designed using **CQRS (Command Query Responsibility Segregation)** with **.NET 8**. The API handles **meal reservations, menu management, stock tracking, and order processing** with robust authentication and auditing.

## 🚀 Features  
✅ **CQRS Architecture** – Implements **MediatR** for clear separation of commands and queries  
✅ **Dapper for Data Access** – Optimized SQL queries for fast database interactions  
✅ **Authentication & Authorization** – Secure JWT-based authentication with refresh token support  
✅ **FluentValidation** – Input validation for reliable API requests  
✅ **AutoMapper** – Efficient DTO mapping for better maintainability  
✅ **Serilog Logging** – Centralized structured logging for debugging and analytics  
✅ **NPOI for Excel Reporting** – Export order and payment reports in Excel format  
✅ **Audit Logging** – Tracks changes for accountability  

## 🛠️ Tech Stack  
- **Backend:** .NET 8, C#, ASP.NET Core Web API  
- **Architecture:** CQRS (MediatR)  
- **Data Access:** Dapper (Micro-ORM)  
- **Authentication:** JWT + Refresh Tokens  
- **Database:** PostgreSQL 17  
- **Logging:** Serilog  
- **Validation:** FluentValidation  
- **Documentation:** Swashbuckle (Swagger)  

## 📂 Project Structure  
```
📦 Ncs.Cqrs
 ├── 📂 Ncs.Cqrs.Api          # Presentation layer (Controllers)
 ├── 📂 Ncs.Cqrs.Application  # Application layer (CQRS: Commands, Queries, DTOs, Validators)
 ├── 📂 Ncs.Cqrs.Domain       # Domain entities and interfaces
 ├── 📂 Ncs.Cqrs.Infrastructure # Database access (Dapper repositories, migrations)
 ├── 📂 Ncs.Cqrs.Common       # Shared utilities and constants
```

## ⚡ Getting Started  
1️⃣ **Clone the repository:**  
```sh
git clone https://github.com/yourusername/Ncs.Cqrs.git
```
2️⃣ **Navigate to the project directory:**  
```sh
cd Ncs.Cqrs
```
3️⃣ **Configure your database connection (PostgreSQL 17)** in `appsettings.json`  
4️⃣ **Start the API:**   
```sh
dotnet run --project Ncs.Cqrs.Api
```

## 📝 API Documentation  
Swagger UI is available at:  
```
http://localhost:5000/swagger
```

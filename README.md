# 🏢 NCS Cqrs API

**CQRS-based REST API using .NET 8, Dapper, and MediatR**

## 🔥 Overview  
NCS Cqrs API is a high-performance **REST API** designed using **CQRS (Command Query Responsibility Segregation)** with **.NET 8**. The API handles **meal reservations, menu management, stock tracking, and order processing** with robust authentication and auditing.

---

## 🚀 Features  
- ✅ **CQRS Architecture** – Implements **MediatR** for clear separation of commands and queries  
- ✅ **Dapper for Data Access** – Optimized SQL queries for fast database interactions  
- ✅ **Authentication & Authorization** – Secure JWT-based authentication with refresh token support  
- ✅ **FluentValidation** – Input validation for reliable API requests  
- ✅ **AutoMapper** – Efficient DTO mapping for better maintainability  
- ✅ **Serilog Logging** – Centralized structured logging for debugging and analytics  
- ✅ **NPOI for Excel Reporting** – Export order and payment reports in Excel format  
- ✅ **Audit Logging** – Tracks changes for accountability  

---

## 🛠️ Tech Stack  
- **Backend:** .NET 8, C#, ASP.NET Core Web API  
- **Architecture:** CQRS (MediatR)  
- **Data Access:** Dapper (Micro-ORM)  
- **Authentication:** JWT + Refresh Tokens  
- **Database:** PostgreSQL 17  
- **Logging:** Serilog  
- **Validation:** FluentValidation  
- **Documentation:** Swashbuckle (Swagger)  

---

## 📂 Project Structure  
```
📦 Ncs.Cqrs
 ├── 📂 Ncs.Cqrs.Api          # Presentation layer (Controllers)
 ├── 📂 Ncs.Cqrs.Application  # Application layer (CQRS: Commands, Queries, DTOs, Validators)
 ├── 📂 Ncs.Cqrs.Domain       # Domain entities and interfaces
 ├── 📂 Ncs.Cqrs.Infrastructure # Database access (Dapper repositories, migrations)
 ├── 📂 Ncs.Cqrs.Common       # Shared utilities and constants
```

---

## 🚀 Getting Started  
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

---

## 📝 API Documentation  
Swagger UI is available at:  
```
http://localhost:5000/swagger
```

---

# 🛠️ Error Handling in NCS Cqrs API

NCS Cqrs API implements a **centralized error handling mechanism** to ensure consistency and maintainability. This architecture ensures that all errors are **caught, logged, and returned with a structured response format**.

---

## 📌 Error Handling Architecture (Mechanism)

The API follows a **layered architecture** to handle errors efficiently:  

### **🔹 1. Error Occurs in Any Layer**
   - **Business Logic Layer (Application Services)**
   - **Database Layer (Dapper Queries)**
   - **External Services (e.g., RFID readers, Payment Gateway)**
   - **Middleware (Authentication & Authorization)**

### **🔹 2. Exception is Thrown**
   - If an **exception occurs**, it is caught in the `HandleRequestAsync<T>` method inside `BaseApiController`.

### **🔹 3. Exception is Logged**
   - The error message, stack trace, and request details (if available) are logged using **Serilog**.
   - **Sensitive data (e.g., passwords, tokens) are never logged**.

### **🔹 4. API Returns a Standardized Error Response**
   - **HTTP Status Code** is set based on the error type.
   - **JSON Response** follows a consistent format with an `errorCode`, `message`, and `messageDetail`.

---

## 📈 Error Handling Flow Diagram
```
[API Request] --> [Controller] --> [Service Layer] --> [Database / External Service]
                    |                   |
                    |                   |
                    V                   V
              [Exception Occurs]  --> [Exception Thrown]
                    |
                    V
        [HandleRequestAsync<T> in BaseApiController]
                    |
                    V
      [Log Error (Serilog)]  -->  [Return JSON Response]
```

---

## 👌 Standard API Response Format

#### **🔹 Success Response**
```json
{
  "success": true,
  "errorCode": null,
  "message": "Data retrieved successfully.",
  "messageDetail": null,
  "data": { }
}
```

#### **🔹 Error Response**
```json
{
  "success": false,
  "errorCode": "NotFound",
  "message": "The requested resource was not found.",
  "messageDetail": "No menu item found with ID 123.",
  "data": null
}
```

---

## 🚀 How to Test Error Handling
- **Invalid Requests:** Send requests with missing or incorrect parameters.
- **Unauthorized Requests:** Call a protected endpoint without a valid token.
- **Database Errors:** Request a non-existing record.

---

## 🔒 Summary
- **Centralized error handling ensures consistency** ✅
- **All exceptions are logged with request details** ✅
- **Sensitive data is never logged** ✅
- **Predefined error codes improve API debugging** ✅

# 🔒 Reports & Exporting

NCS Cqrs API provides **reporting capabilities** using **NPOI** to generate Excel reports for **orders, payments, and stock management**.

## 📈 Available Reports
- **Order Reports** – Export user meal reservations and order status.
- **Payment Reports** – Generate payroll deductions for meal payments.
- **Stock Reports** – Track daily stock levels and consumption.

## 📑 Generating Reports
Reports can be generated using the `/api/reports/export` endpoint. The format is as follows:

### **🔹 Request:**
```http
GET /api/reports/export?reportType=orders&startDate=2024-01-01&endDate=2024-01-31
```

### **🔹 Response:** (Excel File)
The API returns an **Excel file** containing the requested report. The file is dynamically generated based on **real-time** data.

## 🔒 Security & Access Control
- Only **authenticated admins** can generate reports.
- Reports are **not cached** to ensure real-time accuracy.


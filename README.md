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
- ✅ **API Versioning** – Multiple API versions (`v1.0`, `v2.0`, etc.) for backward compatibility  

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
- **Versioning:** Microsoft.AspNetCore.Mvc.Versioning  

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
# 📌 API Versioning  

NCS Cqrs API supports **multiple API versions** to ensure backward compatibility while allowing new features to be added without breaking existing functionality.  

### ✅ **Versioning Strategies Used**  
- **URL Path Versioning** → `/api/v1/orders`, `/api/v2/orders`  
- **Query String Versioning** → `/api/orders?api-version=1.0`  
- **Header Versioning** → `X-API-Version: 1.0`  

### 🛠 **How API Versioning is Implemented**  
The API uses `Microsoft.AspNetCore.Mvc.Versioning` to manage multiple versions.  

#### **1️⃣ Versioned API Controllers**  
Each API controller has a version assigned to it:  

##### **OrdersController (Version 1.0)**
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
[ApiVersion("1.0")]
public class OrdersController : BaseApiController
{
    [HttpGet]
    public IActionResult GetOrders()
    {
        return Ok(new { Message = "Orders from v1 API" });
    }
}
```

##### **OrdersController (Version 2.0 - New Features)**
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
[ApiVersion("2.0")]
public class OrdersV2Controller : BaseApiController
{
    [HttpGet]
    public IActionResult GetOrders()
    {
        return Ok(new { Message = "Orders from v2 API - New Features!" });
    }
}
```

#### **2️⃣ Calling Different API Versions**  
| **Versioning Method** | **Example Request** |
|-------------------|---------------------------------|
| **URL Versioning** | `GET /api/v1/orders` |
| **Query String Versioning** | `GET /api/orders?api-version=1.0` |
| **Header Versioning** | `X-API-Version: 1.0` |

---

## 🔄 **Deprecating an API Version**  
Older API versions can be marked as **deprecated** to notify clients:  

```csharp
[ApiVersion("1.0", Deprecated = true)]
```
Clients will receive a warning indicating that **v1.0 is deprecated**.

---

## 🚀 How to Test API Versioning  
1️⃣ **Start the API:**  
```sh
dotnet run --project Ncs.Cqrs.Api
```
2️⃣ **Test versioned endpoints using URL:**  
```sh
curl -X GET "http://localhost:5000/api/v1/orders"
curl -X GET "http://localhost:5000/api/v2/orders"
```
3️⃣ **Test using Query String Versioning:**  
```sh
curl -X GET "http://localhost:5000/api/orders?api-version=1.0"
```
4️⃣ **Test using Header Versioning:**  
```sh
curl -X GET "http://localhost:5000/api/orders" -H "X-API-Version: 1.0"
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

### **🔒 Security in NCS Cqrs API**

NCS Cqrs API is built with **security-first principles**, ensuring **secure authentication, authorization, and data protection**. The system leverages **CORS policies**, **JWT-based authentication**, and **secure token management** to protect API endpoints.

---

## **1️⃣ CORS (Cross-Origin Resource Sharing)**
**CORS (Cross-Origin Resource Sharing)** is a security mechanism that **controls which domains can access the API**. By default, web browsers enforce CORS policies to prevent **cross-site request forgery (CSRF) attacks**.

### **🔹 CORS Configuration in NCS Cqrs API**
To allow **specific frontend applications** to access the API securely, CORS is configured in `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder.WithOrigins("https://your-frontend.com")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials());
});
```

✅ **Only `https://your-frontend.com` can make requests**  
✅ **Supports all HTTP methods (`GET`, `POST`, `PUT`, `DELETE`)**  
✅ **Ensures cookies and authentication headers are included**  

### **🔹 Enabling CORS in the Middleware**
```csharp
app.UseCors("AllowSpecificOrigins");
```

---

## **2️⃣ Authentication: JWT Token-Based Security**
NCS Cqrs API uses **JSON Web Tokens (JWT)** for **authentication**. Every request must include a valid **Access Token** in the `Authorization` header.

### **🔹 How JWT Authentication Works**
1. **User logs in** → Receives an `access_token` and `refresh_token`
2. **User makes API requests** → Includes `access_token` in `Authorization` header
3. **API verifies token** → Grants or denies access
4. **If token expires** → User must **refresh** the token using `refresh_token`
5. **Refresh Token Rotates** → Ensures long-term security

### **🔹 JWT Token Configuration in `appsettings.json`**
```json
{
  "JwtSettings": {
    "Secret": "super_secure_key_change_this",
    "Issuer": "https://your-api.com",
    "Audience": "https://your-frontend.com",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

### **🔹 Securing API Endpoints with JWT**
By default, API controllers require **JWT authentication**:

```csharp
[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetOrders()
    {
        return Ok(new { Message = "Secure Orders Data" });
    }
}
```
✅ **Only authenticated users can access this endpoint**  
✅ **Unauthorized requests receive `401 Unauthorized` error**  

---

## **3️⃣ JWT Token Lifecycle**
### **📌 Access Token**
- Short-lived (expires in **15 minutes** by default)
- Used for **API authorization**
- Included in **every request** in the `Authorization` header:

```http
Authorization: Bearer <access_token>
```

### **📌 Refresh Token**
- Long-lived (expires in **7 days** by default)
- Used to **request a new access token** without re-login
- Stored securely in **database (`users.refresh_token`)**

#### **🔹 Refresh Token Flow**
1. **Access token expires** (`401 Unauthorized`)
2. **Client sends refresh token** → Calls `/api/auth/refresh-token`
3. **API verifies refresh token** → Issues a **new access token**
4. **Refresh token rotates** (old one is invalidated)

---

## **4️⃣ Refresh Token Endpoint**
📄 **`AuthController.cs`**
```csharp
[HttpPost("refresh-token")]
[AllowAnonymous]
public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
{
    var response = await _mediator.Send(command);
    return Ok(response);
}
```

✅ **Prevents forced logouts by using refresh tokens**  
✅ **Enhances security with token rotation**  

### **🔹 Example API Calls**
#### **🔹 1️⃣ Login (Receive Tokens)**
```http
POST /api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "admin@company.com",
  "password": "SecurePass123"
}
```
✅ **Response (Contains `access_token` and `refresh_token`)**
```json
{
  "token": "eyJhbGciOiJIUzI1Ni...",
  "refreshToken": "i2Kx89YHsaF...",
  "expiresIn": 15
}
```

#### **🔹 2️⃣ Access Protected Resource**
```http
GET /api/orders
Authorization: Bearer <access_token>
```

#### **🔹 3️⃣ Refresh Expired Token**
```http
POST /api/auth/refresh-token
Content-Type: application/json

{
  "accessToken": "<expired_access_token>",
  "refreshToken": "<valid_refresh_token>"
}
```
✅ **Response (New Tokens)**
```json
{
  "token": "new_jwt_access_token",
  "refreshToken": "new_refresh_token",
  "expiresIn": 15
}
```

---

## **5️⃣ Securing API with Role-Based Authorization**
NCS Cqrs API enforces **role-based access control (RBAC)** using JWT claims.

📄 **`OrdersController.cs`**
```csharp
[Authorize(Roles = "Admin")]
[HttpPost("create")]
public IActionResult CreateOrder()
{
    return Ok(new { Message = "Order Created Successfully" });
}
```
✅ **Only users with the `Admin` role can create orders**  
✅ **Other users receive a `403 Forbidden` response**  

---

## **6️⃣ Security Best Practices in NCS Cqrs API**
✅ **Use HTTPS** – Prevents token interception  
✅ **Token Expiry & Rotation** – Prevents replay attacks  
✅ **Store Refresh Tokens in Database** – Prevents reuse attacks  
✅ **Role-Based Access Control** – Restricts access to authorized users  
✅ **CORS Policy** – Prevents unauthorized cross-origin requests  
✅ **Centralized Logging (Serilog)** – Detects suspicious activity  

---

## **🚀 Summary: API Security Features**
| **Feature** | **Description** |
|------------|---------------|
| **CORS Policy** | Prevents unauthorized cross-origin requests |
| **JWT Access Token** | Short-lived tokens for API authorization |
| **Refresh Token** | Securely stored, used to obtain new access tokens |
| **Token Rotation** | Refresh tokens are invalidated after use |
| **Role-Based Authorization** | Access control using JWT roles |
| **Secure Password Hashing** | User passwords stored using **PBKDF2** |
| **HTTPS Enforcement** | Ensures encrypted API communication |

---

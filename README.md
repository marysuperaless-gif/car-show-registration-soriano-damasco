# Car Show Registration System

Course: Integrative Programming 2 - CCDI Sorsogon

| MEMBER | ROLE |
| ------ | ---- |
| mary soriano | Backend Developer |
| justine damasco | Frontend Developer |

## Backend (Car Show Registration API)

REST API para sa Car Show 2027 registration. Gumagamit ito ng *C# / .NET 10*, *Vertical Slice Architecture*, *EF Core + SQLite*, *FluentValidation* at *Scalar*.

### Requirements
- .NET 10 SDK
- dotnet-ef tool (para sa migrations): dotnet tool install --global dotnet-ef

### Project structure
backend/
  Api/              Endpoints, Program.cs, ValidationFilter, Api.http
  Application/      Features/Registrations (4 slices), abstractions
  Domain/           Registration entity, enums, Result<T>
  Infrastructure/   AppDbContext, DbSeeder, Migrations (SQLite)

Mga slice sa Application/Features/Registrations:
- CreateRegistration (command, handler, validator)
- GetRegistrations (query, handler, validator)
- GetRegistrationById (query, handler)
- GetRegistrationSummary (query, handler)

Endpoints, Program.cs, ValidationFilter, Api.http
  Application/        Features/Registrations (4 slices), abstractions
  Domain/            Registration entity, enums, Result<T>
  Infrastructure/     AppDbContext, DbSeeder, Migrations (SQLite)


The slices under Application/Features/Registrations:
- CreateRegistration (command, handler, validator)
- GetRegistrations (query, handler, validator)
- GetRegistrationById (query, handler)
- GetRegistrationSummary (query, handler)

### Endpoints
| Method | Endpoint | Description | Success | Errors |
|---|---|---|---|
| POST | /api/registrations | Create a registration | 201 | 400, 409 |
| GET | /api/registrations | List registrations (paged, with filters) | 200 | 400 |
| GET | /api/registrations/{id} | Find registration by ID | 200 | 404 |
| GET | /api/registrations/summary | KPI totals for the dashboard | 200 | None |

Query parameters for GET /api/registrations:
- participantName – contains search, case-insensitive
- type – Exhibitor, Competitor, VIP
- status – Pending, Confirmed, CheckedIn, Cancelled
- page – default 1
- pageSize – default 10, range 1 to 50
Results are sorted by registrationDate, newest first.

### Business Rules
- Fees by type: Exhibitor 1,500; Competitor 2,500; VIP 5,000. The server determines the fee.
- Registration number (CS27-0001, etc.), date (UTC), and status (Pending) are assigned by the server. Client-supplied values for these fields are ignored.
- Plate number must be unique. Duplicate plate numbers return 409 Conflict.
- amountPaid must be between 0 and the fee for the selected type.
- paymentStatus is computed (not stored): Unpaid, Partial, or Paid.
- Cancelled registrations are excluded from the summary's monetary totals.

### Status Codes
- 400 – Invalid input (ProblemDetails with errors)
- 404 – No registration found with the specified ID
- 409 – Duplicate plate number

### Database
- SQLite (backend/Api/carshow.db). The database is automatically created and seeded on first run (Database.Migrate() and DbSeeder).
- 25 sample registrations are seeded, covering all types, statuses, and payment statuses.
- Unique indexes on PlateNumber and RegistrationNumber.
- To reset the data: stop the API, delete carshow.db*, and run it again.

### Run
cd backend
dotnet restore
dotnet run --project Api


- API: http://localhost:5205
- Scalar: http://localhost:5205/scalar/v1
- CORS allows the React dev server at http://localhost:5173.


### Migrations
cd backend
dotnet ef migrations add <Name> --project Infrastructure --startup-project Api --output-dir Migrations


### Testing
- Open Scalar and test each endpoint.
- Alternatively, use backend/Api/Api.http (Cases 01 to 37) in Visual Studio.
- Test screenshots are located in docs/screenshots/.

### JSON Contract
Field names use camelCase, and enums are represented as strings ("Competitor", "Pending", "Partial"). The exact request and response shapes follow the agreement with the Frontend Developer.


## Run the frontend
```bash
cd frontend
npm install
npm run dev
```


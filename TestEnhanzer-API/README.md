# TestEnhanzer — Full Stack Developer Assignment (Angular + .NET Core)

A two-page web application built for the Enhanzer "Full Stack Developer – Angular & .NET Core" assignment:

1. **Login page** – authenticates against the external Enhanzer POS API, issues a JWT, and persists the returned user locations to SQL Server.
2. **Purchase Bill page** (protected) – add purchase-bill items with autocomplete, batch selection, live calculations, an items table, and a summary.

| Layer     | Technology                                   |
| --------- | -------------------------------------------- |
| Frontend  | Angular 22 (standalone components + signals) |
| Backend   | ASP.NET Core Web API (.NET 9), C#            |
| Database  | SQL Server (via EF Core code-first)          |
| Auth      | JWT bearer tokens                            |

---

## Repository layout

```
TestEnhanzer/
├─ TestEnhanzer.sln              # .NET solution
├─ TestEnhanzer/                 # ASP.NET Core Web API project
│  ├─ Controllers/               # Auth, Locations, Items, PurchaseBills
│  ├─ Services/                  # Auth, POS API client, token, calculator
│  ├─ Data/                      # EF Core DbContext
│  ├─ Models/                    # Entities, DTOs, external API models
│  └─ Migrations/                # EF Core migrations
├─ TestEnhanzer-APP/             # Angular 22 application
│  └─ src/app/
│     ├─ core/                   # services, guards, interceptors, models
│     ├─ shared/components/      # reusable UI (autocomplete, alert, spinner)
│     └─ features/               # login + purchase-bill pages
├─ database/
│  ├─ CreateDatabase.sql         # standalone SQL Server creation script
│  └─ schema.sql                 # EF Core idempotent migration script
└─ README.md
```

---

## Prerequisites

- **.NET SDK 9** (or 8) — <https://dotnet.microsoft.com/download>
- **Node.js ≥ 22.22.3** (required by the Angular 22 CLI) and npm
- **SQL Server** — LocalDB, Express, or full. The default connection string uses LocalDB
  (`(localdb)\MSSQLLocalDB`), which ships with Visual Studio.

---

## 1. Database setup

The API creates the database and `Location_Details` table automatically on first
start (via EF Core migrations). If you prefer to create it manually, run one of the
scripts in the `database/` folder against your SQL Server instance:

```sql
-- Full standalone script (creates the database + table)
:r database/CreateDatabase.sql
```

To point at a different SQL Server, edit `TestEnhanzer/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=TestEnhanzerDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

## 2. Run the backend (API)

```bash
cd TestEnhanzer
dotnet restore
dotnet run --launch-profile http
```

The API starts on **http://localhost:5139**. In `Development` it does **not** force
HTTPS, so the Angular dev server can call it over HTTP.

Key endpoints:

| Method | Route                          | Auth | Description                                   |
| ------ | ------------------------------ | ---- | --------------------------------------------- |
| POST   | `/api/auth/login`              | –    | Authenticates via the POS API, returns a JWT  |
| GET    | `/api/locations`               | ✅   | Saved locations for the batch dropdown        |
| GET    | `/api/items?search=`           | ✅   | Autocomplete item list                        |
| POST   | `/api/purchasebills/calculate` | ✅   | Calculates totals for one line                |
| POST   | `/api/purchasebills`           | ✅   | Calculates all lines + summary                |

> **Security note:** change the `Jwt:Key` value in `appsettings.json` before any
> real deployment. It is set to a placeholder secret for local development only.

---

## 3. Run the frontend (Angular)

```bash
cd TestEnhanzer-APP
npm install
npm start
```

The app starts on **http://localhost:4200** and calls the API at
`http://localhost:5139/api` (configured in `src/environments/environment.development.ts`).
CORS on the backend already allows `http://localhost:4200`.

---

## How it works

### Login (Task 1)

- The client posts `{ email, password }` to `/api/auth/login`.
- The backend calls the external POS API with the flat payload
  `{ Company_Code, Username, Pw }` where **Company_Code** and **Username** are both
  the email, and **Pw** is the password (per the assignment).
- On success the response's `User_Locations` array (`Location_Code`, `Location_Name`)
  is saved to the `Location_Details` table, a JWT is issued, and the client stores the
  session. The Purchase Bill route is protected by an Angular route guard and the JWT
  bearer `[Authorize]` attribute on the API.

### Purchase Bill (Task 2)

- **Item** – reusable autocomplete over: Mango, Apple, Banana, Orange, Grapes, Kiwi, Strawberry.
- **Batch** – dropdown populated from the saved `Location_Details` (`Location_Name`).
- **Calculations**
  - `Total Cost = (Standard Cost × Quantity) − Discount%` → e.g. `100 × 5 − 20% = 400`
  - `Total Selling = Standard Price × Quantity` → e.g. `150 × 5 = 750`
- **On Add** – the line is added to the table and the summary updates:
  - `Total Items` = number of rows, `Total Quantity` = sum of quantities (plus running cost/selling totals).

---

## Notes

- Component-based, standalone Angular architecture with signals and lazy-loaded routes.
- Reusable UI components (`app-autocomplete`, `app-alert`, `app-spinner`).
- Strong typing across DTOs/models, reactive-form validation, loading indicators, and
  error handling (invalid credentials, unreachable server, unauthorized access).

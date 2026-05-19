# AuraStay Hotel Booking System

A production-style Blazor Server hotel booking system built for the Visual Programming project requirement. It uses a three-tier structure with Razor UI components, service/repository business logic, Entity Framework Core, SQLite persistence, hotel/room seed data, validation, role-aware screens, booking workflow, payments, reviews, and admin analytics.

## Run

```powershell
dotnet restore
dotnet build
dotnet run
```

The database is created automatically as `hotelbooking.db` and seeded on first run.

## Admin Account

| Role | Email | Password |
| --- | --- | --- |
| Admin | admin@aurastay.com | admin123 |

## Key Features

1. Hotel catalogue with 4 seeded hotels.
2. Room inventory with 12 room packages.
3. Search by city, keyword, dates, guests, type, and max price.
4. Availability calculation using overlapping booking logic.
5. Guest registration with validation.
6. Secure SHA-256 password hashing for admin and registered customer accounts.
7. Login/logout session handling.
8. Role-aware navigation and admin access screen.
9. Booking creation with server-side validation.
10. Booking history for guest users.
11. Booking cancellation workflow.
12. Payment capture with transaction reference.
13. Admin dashboard metrics.
14. Room create/update/deactivate workflow.
15. Booking status workflow for admin.
16. Guest reviews and recent review display.
17. Responsive Bootstrap/custom CSS interface.

## Architecture

- UI Layer: `Components/Pages`, `Components/Layout`, `wwwroot/app.css`
- Business Layer: `Services`
- Data Access Layer: `Repositories`, `Data/HotelBookingDbContext.cs`
- Persistence: EF Core SQLite with automatic seed data

See `Documentation/ProjectReport.md` and `Documentation/UML-Class-Diagram.mmd` for submission notes.

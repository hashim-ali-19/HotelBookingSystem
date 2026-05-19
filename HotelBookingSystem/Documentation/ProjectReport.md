# AuraStay Hotel Booking System - Project Report

## Project Overview

AuraStay is a Blazor Server full-stack hotel booking system. The project demonstrates component-based UI, C# OOP, dependency injection, repository/service architecture, Entity Framework Core persistence, validation, authentication-style session management, role-aware screens, reporting, and responsive design.

## Architecture

The application follows a three-tier pattern:

- UI Layer: Razor pages and layout components render hotel discovery, room search, booking forms, account management, and admin screens.
- Business Logic Layer: Services coordinate validation, booking rules, payment workflow, authentication, reporting, and review submission.
- Data Access Layer: Repositories and `HotelBookingDbContext` encapsulate EF Core database operations.

Dependency injection is configured in `Program.cs` for DbContext, repositories, services, and authentication state provider.

## Functional Feature Matrix

| No. | Feature | Evidence |
| --- | --- | --- |
| 1 | Seeded hotel catalogue | Home and Rooms pages |
| 2 | Seeded room inventory | Rooms and Admin pages |
| 3 | Search/filter | Rooms page |
| 4 | Availability logic | `RoomRepository.SearchAvailableRoomsAsync` |
| 5 | Guest registration | Register page |
| 6 | Login/logout | Login and Logout pages |
| 7 | Role-aware navigation | Layout/NavMenu |
| 8 | Booking creation | Rooms page booking form |
| 9 | Booking history | Bookings and Account pages |
| 10 | Booking cancellation | Bookings page |
| 11 | Payment capture | Bookings page and `PaymentService` |
| 12 | Admin dashboard | Admin metrics |
| 13 | Room CRUD | Admin room form |
| 14 | Booking status workflow | Admin booking table |
| 15 | Reviews | Account and Home pages |
| 16 | Reports/analytics | `BookingService.GetDashboardMetricsAsync` |
| 17 | Responsive professional UI | `wwwroot/app.css` |

## OOP and UML Notes

`BaseEntity` is an abstract base class used by `Hotel`, `Room`, `Booking`, `Payment`, `Review`, and `User`. It contains common audit fields and a virtual `GetDisplayName()` method. Derived classes override that method to provide polymorphic display behavior.

Composition/aggregation is used through object collections: a `Hotel` owns many `Room` and `Review` objects, a `User` has many `Booking` and `Review` objects, and a `Booking` has many `Payment` records. These collections satisfy array/object-list usage and show data flow between domain entities.

## Database

The app uses SQLite through EF Core. `DatabaseSeeder.SeedAsync` creates the database on first run and stores:

- 4 hotels
- 12 room packages
- Admin and guest users
- Sample bookings
- Sample payment
- Sample reviews

## Validation and Error Handling

Data annotations validate forms on the client-side Blazor form layer and again in service methods. Booking services check login state, room existence, capacity, selected dates, availability, and authorization before writing data.

## Deployment Guide

1. Install .NET 10 SDK.
2. Open the solution in Visual Studio.
3. Restore NuGet packages.
4. Build the project.
5. Run the project.
6. Login with the admin account in `README.md`, or register a new customer account through the app.

## User Manual

1. Open Home to view hotels and metrics.
2. Go to Rooms to search available rooms.
3. Login as guest, select a room, and confirm booking.
4. Open Bookings to pay or cancel.
5. Open Account to view profile and submit a review.
6. Login as admin to access dashboard, room management, and booking workflow.

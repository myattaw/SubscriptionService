# SubscriptionService

## Overview
SubscriptionService is a backend API built with ASP.NET Core that simulates a subscription and billing system. It manages users, subscription plans, and recurring billing logic using clean architecture and asynchronous processing.

This project focuses on backend system design, business logic, and API development rather than frontend UI.

---

## Features
- Create and manage users
- Assign and manage subscription plans
- Simulate billing cycles (monthly/yearly)
- Process payments (simulated success/failure)
- Background worker for recurring billing processing
- RESTful API endpoints

---

## Tech Stack
- C#
- ASP.NET Core Web API
- .NET 8.0
- Entity Framework Core (optional)
- Swagger (OpenAPI)

---

## Project Structure
- Controllers – API endpoints
- Services – Business logic and processing
- Models – Data models
- Background Workers – Async billing processing

---

## Example Endpoints
- POST /api/users
- POST /api/subscriptions
- GET /api/subscriptions/{userId}
- POST /api/subscriptions/{id}/cancel
- POST /api/billing/run

---

## Running the Project
1. Open the solution in JetBrains Rider or Visual Studio
2. Build the project
3. Run the application
4. Navigate to:
   https://localhost:{port}/swagger

---

## Notes
This project simulates payment processing and does not integrate with real payment providers. The focus is on backend system design and business logic.

---

## Purpose
This project was created to demonstrate backend engineering skills, including API design, asynchronous processing, and system architecture using .NET technologies.
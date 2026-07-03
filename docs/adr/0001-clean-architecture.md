# ADR 0001: Use Clean Architecture for Backend

## Context
The backend needs to support multiple modules (Patient, Billing, Pharmacy, etc.)
that will grow over time (14 modules planned long-term). Business logic must stay
independent of frameworks so it can be tested and evolved safely.

## Decision
Use Clean Architecture with 4 layers: Domain, Application, Infrastructure, API.
Domain has zero dependencies. Application depends only on Domain. Infrastructure
and API depend inward.

## Consequences
+ Business rules are testable without a database or web server
+ Easy to swap infrastructure (e.g., change ORM) without touching business logic
- More initial boilerplate than a simple layered/MVC approach
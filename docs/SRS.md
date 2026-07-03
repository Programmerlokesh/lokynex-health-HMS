# Lokynex Health — Software Requirements Specification (MVP)

## 1. Problem Statement
Small-to-mid Indian hospitals/clinics lack an affordable, modern, multi-tenant HMS.
Lokynex Health solves this by offering a SaaS platform where one company (SuperAdmin)
sells software access to multiple hospitals (tenants).

## 2. Users
- SuperAdmin (platform owner) — manages tenants, billing, module access
- Hospital Admin — manages their facility's staff and modules
- Doctor, Receptionist, Pharmacist, Accountant — day-to-day operational staff
- Patient — registered and treated within a tenant

## 3. In Scope (MVP)
- SuperAdmin: tenant onboarding, module activation, subscription tracking
- Patient Registration module
- Doctor / OPD module
- Billing & Finance module
- Pharmacy POS module
- Ward & Bed Management module

## 4. Out of Scope (Phase 2 — post-employment)
- ICU Monitoring, Emergency/ER, OT Management, Blood Bank,
  Radiology/PACS, HR & Payroll, Patient Portal & Telemedicine

## 5. Success Criteria
- A SuperAdmin can onboard a new tenant and activate modules
- A patient can be registered, seen by a doctor, prescribed medicine,
  billed, and (if needed) admitted to a ward — end to end, in one demo flow
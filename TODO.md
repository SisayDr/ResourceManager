# Resource Manager - TODO

## 1. Project Setup

- [x] Create backend folder with .NET Web API project
- [x] Set up Git repository
- [x] Add `.gitignore`
- [x] Add basic project README

## 2. Database & Models

- [x] Install Entity Framework SqlServer & Tools
- [x] Configure `DbContext`
- [x] Create `Group` model
- [x] Create `User` model
- [x] Create `ResourceType` model
- [x] Create `Resource` model
- [x] Create `Reservation` model
- [x] Define model relationships
- [x] Define required fields and validation
- [x] Add migrations and update database
- [ ] Seed initial data

## 3. Group Management

- [x] Implement Group Service CRUD [5/5]
- [x] Implement Group routes
- [x] Test Group CRUD

## 4. User Management

- [x] Implement User Service CRUD [5/5]
- [x] Implement User routes
- [x] Add request/response DTOs
- [x] Test User CRUD

## 5. Resource Types

- [x] Implement ResourceType Service CRUD [5/5]
- [x] Implement ResourceType routes
- [x] Add resource type validation
- [x] Prevent deletion when resources depend on a resource type
- [x] Test ResourceType CRUD

## 6. Resources

- [x] Implement Resource Service CRUD [0/5]
- [x] Implement Resource routes
- [ ] Add resource validation
- [ ] Add resource open hours
- [x] Test Resource CRUD
- [x] Prevent deletion when active reservations exist

## 7. Reservations

- [ ] Implement Reservation Service CRUD [0/5]
- [ ] Implement Reservation routes
- [ ] Prevent reservations in the past
- [ ] Prevent overlapping reservations
- [ ] Prevent reservations outside resource open hours
- [ ] Test Reservation CRUD

## 8. Search / Availability

- [ ] `GET /api/resources/availability?type=&date=&duration=` - Search available resources by type
- [ ] `GET /api/resources/{id}/availability?date=&duration=` - Check availability of a specific resource

## 9. Authentication & Authorization

- [x] Implement login/logout endpoint
- [x] Implement user authentication
- [x] Add authentication middleware
- [x] Add authorization policies/roles
- [x] Test unauthorized requests
- [x] Test forbidden requests

## 10. Frontend

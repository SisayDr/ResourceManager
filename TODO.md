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

- [ ] Implement Group Controller CRUD
- [ ] Implement Group routes
- [ ] Test Group CRUD

## 4. User Management

- [ ] Implement User Controller CRUD [0/5]
- [ ] Implement User routes
- [ ] Add request/response DTOs
- [ ] Add user validation
- [ ] Test User CRUD
- [ ] Handle duplicate users

## 5. Resource Types

- [ ] Implement ResourceType Controller CRUD [0/5]
- [ ] Implement ResourceType routes
- [ ] Add resource type validation
- [ ] Prevent deletion when resources depend on a resource type
- [ ] Test ResourceType CRUD

## 6. Resources

- [ ] Implement Resource Controller CRUD [0/5]
- [ ] Implement Resource routes
- [ ] Add resource validation
- [ ] Add resource open hours
- [ ] Test Resource CRUD
- [ ] Prevent deletion when active reservations exist

## 7. Reservations

- [ ] Implement Reservation Controller CRUD [0/5]
- [ ] Implement Reservation routes
- [ ] Prevent reservations in the past
- [ ] Prevent overlapping reservations
- [ ] Prevent reservations outside resource open hours
- [ ] Test Reservation CRUD

## 8. Search / Availability

- [ ] `GET /api/resources/availability?type=&date=&duration=` - Search available resources by type
- [ ] `GET /api/resources/{id}/availability?date=&duration=` - Check availability of a specific resource

## 9. Authentication & Authorization

- [ ] Implement login/logout endpoint
- [ ] Implement user authentication
- [ ] Add authentication middleware
- [ ] Add authorization policies/roles
- [ ] Test unauthorized requests
- [ ] Test forbidden requests

## 10. Frontend

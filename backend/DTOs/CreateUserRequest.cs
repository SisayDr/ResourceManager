namespace ResourceManagerAPI.DTOs;

public record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    string Role
);


namespace ResourceManagerAPI.DTOs;
public record UserResponse(
    string Id,
    string FullName,
    string Email,
    string? Role
);

namespace ResourceManagerAPI.DTOs;

public record LoginRequest(
    string Email,
    string Password
);
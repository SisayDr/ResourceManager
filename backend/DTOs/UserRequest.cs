namespace ResourceManagerAPI.DTOs;

public record UserRequest(
    string FullName,
    string Email,
    string Password,
    string Role,
    Guid? GroupId

);
public record UserUpdateRequest(
    string FullName,
    string Email,
    string? Password,
    string? Role,
    Guid? GroupId
);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

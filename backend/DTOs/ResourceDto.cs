namespace ResourceManagerAPI.DTOs;

public record ResourceRequest(
    string Name,
    int TotalCapacity,
    ReservationMode ReservationMode,
    Guid ResourceTypeId,
    Guid GroupId
);

public record ResourceResponse(
    Guid Id,
    string Name,
    int TotalCapacity,
    ReservationMode ReservationMode,
    Guid ResourceTypeId,
    Guid GroupId
);

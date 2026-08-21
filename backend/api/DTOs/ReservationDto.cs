namespace ResourceManagerAPI.DTOs;

public record ReservationRequest(
    DateTimeOffset Start,
    DateTimeOffset End,
    int BookedCapacity,
    ReservationStatus? Status,
    Guid ResourceId
);
public record ReservationResponse(
    Guid Id,
    DateTimeOffset Start,
    DateTimeOffset End,
    int BookedCapacity,
    ReservationStatus? Status,
    string ResourceName,
    string ReservedBy
);
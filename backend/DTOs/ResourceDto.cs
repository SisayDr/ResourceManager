namespace ResourceManagerAPI.DTOs;

public enum ReservationMode { shared, exclusive }
public record ResourceDto(
    string Name,
    int TotalCapacity,
    ReservationMode ReservationMode,
    Guid ResourceTypeId,
    Guid GroupId
);

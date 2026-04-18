namespace reservations_api.DTOs.Responses;

public class GetReservationsByDateResponse
{
    public List<ReservationDto> Reservations { get; set; } = new();
    public int Total { get; set; }
    public DateOnly Date { get; set; }
}

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid ClassroomId { get; set; }
    public string ClassroomName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
namespace reservations_api.DTOs.Responses;

public class DeleteReservationResponse
{
    public bool Success { get; set; }    
    public string Message { get; set; } = string.Empty; 
    public Guid ReservationId { get; set; } 
}
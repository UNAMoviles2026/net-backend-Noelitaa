using reservations_api.DTOs.Requests;
using reservations_api.DTOs.Responses;
using reservations_api.Mappers;
using reservations_api.Models.Entities;
using reservations_api.Repositories;


namespace reservations_api.Services;

public class ReservationService : IReservationService
{
  private readonly IReservationRepository _reservationRepository;
  private readonly IClassroomRepository _classroomRepository;  


  public ReservationService(IReservationRepository reservationRepository, IClassroomRepository classroomRepository)
  {
    _reservationRepository = reservationRepository;
    _classroomRepository = classroomRepository;  

  }

  public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request)
  {
    if (request.StartTime >= request.EndTime)
    {
      throw new InvalidOperationException("StartTime must be less than EndTime");
    }

    var existingReservations = await _reservationRepository.GetByClassroomAndDateAsync(
        request.ClassroomId,
        request.Date);

    if (HasOverlap(request.StartTime, request.EndTime, existingReservations))
    {
      throw new InvalidOperationException("Time conflict with another reservation");
    }

    var reservation = ReservationMapper.ToEntity(request);
    var createdReservation = await _reservationRepository.AddAsync(reservation);

    return ReservationMapper.ToResponse(createdReservation);
  }

  private static bool HasOverlap(TimeOnly startTime, TimeOnly endTime, List<Reservation> existingReservations)
  {
    return existingReservations.Any(r =>
        startTime < r.EndTime && endTime > r.StartTime);
  }

  public async Task<GetReservationsByDateResponse> GetByDateAsync(DateOnly date)
{
    var reservations = await _reservationRepository.GetByDateAsync(date);
    
    var reservationDtos = new List<ReservationDto>();
    
    foreach (var reservation in reservations)
    {
        var classroom = await _classroomRepository.GetByIdAsync(reservation.ClassroomId);
        
        reservationDtos.Add(new ReservationDto
        {
            Id = reservation.Id,
            ClassroomId = reservation.ClassroomId,
            ClassroomName = classroom?.Name ?? "Unknown",
            Date = reservation.Date,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime
        });
    }
    
    return new GetReservationsByDateResponse
    {
        Reservations = reservationDtos,
        Total = reservationDtos.Count,
        Date = date
    };
}
}

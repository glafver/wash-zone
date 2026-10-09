using WashZone.Models;

namespace WashZone.Services;

public interface IBookingService
{
    Task<List<Booking>> GetUserBookingsAsync(string userId, bool descending, int? stationId, int? packageId, string? regNumber);
    Task<List<Booking>> GetAllBookingsAsync(bool descending, int? stationId, int? packageId, string? regNumber, string? phoneNumber);
    Task<Booking?> GetBookingAsync(int id);
    Task<List<Booking>> GetBookingsForStationAsync(int stationId, DateTime from, DateTime to);
    Task<bool> IsSlotAvailableAsync(int stationId, DateTime start, int durationMinutes, int? excludeBookingId = null);
    Task<ServiceResult> CreateBookingAsync(string userId, BookingInput input);
    Task<ServiceResult> UpdateBookingAsync(int bookingId, string userId, bool isAdmin, BookingInput input);
    Task<ServiceResult> DeleteBookingAsync(int bookingId, string userId, bool isAdmin);
}

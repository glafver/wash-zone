using Microsoft.EntityFrameworkCore;
using WashZone.Data;
using WashZone.Models;

namespace WashZone.Services;

public class BookingService : IBookingService
{
    private readonly ApplicationDbContext _context;

    public BookingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Booking>> GetUserBookingsAsync(string userId, bool descending, int? stationId, int? packageId, string? regNumber)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Station)
            .Include(b => b.Package)
            .Where(b => b.UserId == userId);

        if (stationId.HasValue) query = query.Where(b => b.StationId == stationId);
        if (packageId.HasValue) query = query.Where(b => b.PackageId == packageId);
        if (!string.IsNullOrWhiteSpace(regNumber)) query = query.Where(b => b.RegistrationNumber.Contains(regNumber));

        query = descending ? query.OrderByDescending(b => b.Date) : query.OrderBy(b => b.Date);

        return await query.ToListAsync();
    }

    public async Task<List<Booking>> GetAllBookingsAsync(bool descending, int? stationId, int? packageId, string? regNumber, string? phoneNumber)
    {
        IQueryable<Booking> query = _context.Bookings
            .Include(b => b.Station)
            .Include(b => b.Package)
            .Include(b => b.User);

        if (stationId.HasValue) query = query.Where(b => b.StationId == stationId);
        if (packageId.HasValue) query = query.Where(b => b.PackageId == packageId);
        if (!string.IsNullOrWhiteSpace(regNumber)) query = query.Where(b => b.RegistrationNumber.Contains(regNumber));
        if (!string.IsNullOrWhiteSpace(phoneNumber)) query = query.Where(b => b.User != null && b.User.PhoneNumber != null && b.User.PhoneNumber.Contains(phoneNumber));

        query = descending ? query.OrderByDescending(b => b.Date) : query.OrderBy(b => b.Date);

        return await query.ToListAsync();
    }

    public Task<Booking?> GetBookingAsync(int id)
        => _context.Bookings.FindAsync(id).AsTask();

    public async Task<List<Booking>> GetBookingsForStationAsync(int stationId, DateTime from, DateTime to)
        => await _context.Bookings
            .Where(b => b.StationId == stationId && b.Date >= from && b.Date < to)
            .OrderBy(b => b.Date)
            .ToListAsync();

    public async Task<ServiceResult> CreateBookingAsync(string userId, BookingInput input)
    {
        if (!await IsSlotAvailableAsync(input.StationId, input.Date, input.DurationMinutes))
        {
            return ServiceResult.Conflict("This time is already booked at the selected station. Please choose another time.");
        }

        _context.Bookings.Add(new Booking
        {
            UserId = userId,
            StationId = input.StationId,
            PackageId = input.PackageId,
            RegistrationNumber = input.RegistrationNumber,
            Date = input.Date,
            DurationMinutes = input.DurationMinutes,
        });

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateBookingAsync(int bookingId, string userId, bool isAdmin, BookingInput input)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return ServiceResult.NotFound("Booking not found.");
        if (booking.UserId != userId && !isAdmin) return ServiceResult.Forbidden();

        if (!await IsSlotAvailableAsync(input.StationId, input.Date, input.DurationMinutes, bookingId))
        {
            return ServiceResult.Conflict("This time is already booked at the selected station. Please choose another time.");
        }

        booking.StationId = input.StationId;
        booking.PackageId = input.PackageId;
        booking.RegistrationNumber = input.RegistrationNumber;
        booking.Date = input.Date;
        booking.DurationMinutes = input.DurationMinutes;

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteBookingAsync(int bookingId, string userId, bool isAdmin)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return ServiceResult.NotFound("Booking not found.");
        if (booking.UserId != userId && !isAdmin) return ServiceResult.Forbidden();

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    /// <summary>
    /// Returns true if the time window [start, start + durationMinutes) is free for the given
    /// station, i.e. it does not overlap any existing booking.
    /// </summary>
    public async Task<bool> IsSlotAvailableAsync(int stationId, DateTime start, int durationMinutes, int? excludeBookingId = null)
    {
        var end = start.AddMinutes(durationMinutes);

        IQueryable<Booking> query = _context.Bookings.Where(b =>
            b.StationId == stationId &&
            b.Date < end &&
            b.Date.AddMinutes(b.DurationMinutes) > start);

        if (excludeBookingId.HasValue) query = query.Where(b => b.Id != excludeBookingId);

        return !await query.AnyAsync();
    }
}

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

    public async Task<ServiceResult> CreateBookingAsync(string userId, BookingInput input)
    {
        if (await IsSlotTakenAsync(input.StationId, input.Date))
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
        });

        await _context.SaveChangesAsync();
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> UpdateBookingAsync(int bookingId, string userId, bool isAdmin, BookingInput input)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return ServiceResult.NotFound("Booking not found.");
        if (booking.UserId != userId && !isAdmin) return ServiceResult.Forbidden();

        if (await IsSlotTakenAsync(input.StationId, input.Date, bookingId))
        {
            return ServiceResult.Conflict("This time is already booked at the selected station. Please choose another time.");
        }

        booking.StationId = input.StationId;
        booking.PackageId = input.PackageId;
        booking.RegistrationNumber = input.RegistrationNumber;
        booking.Date = input.Date;

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

    private Task<bool> IsSlotTakenAsync(int stationId, DateTime date, int? excludeBookingId = null)
    {
        IQueryable<Booking> query = _context.Bookings.Where(b => b.StationId == stationId && b.Date == date);
        if (excludeBookingId.HasValue) query = query.Where(b => b.Id != excludeBookingId);
        return query.AnyAsync();
    }
}

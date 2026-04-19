using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;
        
        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Appointment>> GetAppointmentsByBarberAsync(int barberProfileId, DateTime? date = null)
        {
            var query = _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Where(a => a.BarberProfileId == barberProfileId);

            if (date.HasValue)
            {
                query = query.Where(a => a.Date == date.Value.Date);
            }

            // Order by Time in memory since SQLite doesn't support TimeSpan in ORDER BY
            var appointments = await query.ToListAsync();
            return appointments.OrderBy(a => a.Time).ToList();
        }
        
        public async Task<List<Appointment>> GetAppointmentsByClientAsync(string clientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.BarberProfile)
                .Include(a => a.Service)
                .Where(a => a.ClientId == clientId)
                .ToListAsync();
            
            return appointments
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Time)
                .ToList();
        }
        
        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.BarberProfile)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        
        public async Task<List<Appointment>> GetTodayAppointmentsAsync(int barberProfileId)
        {
            var today = DateTime.Now.Date;
            return await GetAppointmentsByBarberAsync(barberProfileId, today);
        }
        
        public async Task<List<Appointment>> GetUpcomingAppointmentsAsync(int barberProfileId)
        {
            var today = DateTime.Now.Date;
            var appointments = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Where(a => a.BarberProfileId == barberProfileId && a.Date >= today && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
            
            return appointments
                .OrderBy(a => a.Date)
                .ThenBy(a => a.Time)
                .ToList();
        }
        
        public async Task<bool> CreateAppointmentAsync(Appointment appointment, int durationMinutes)
        {
            try
            {
                // Check if the time slot is available
                bool isAvailable = await IsTimeSlotAvailableAsync(appointment.BarberProfileId, appointment.Date, appointment.Time, durationMinutes);

                if (!isAvailable)
                {
                    return false;
                }

                appointment.Status = AppointmentStatus.Pending;
                appointment.CreatedAt = DateTime.Now;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, AppointmentStatus status)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment != null)
                {
                    appointment.Status = status;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            return await UpdateAppointmentStatusAsync(appointmentId, AppointmentStatus.Cancelled);
        }
        
        public async Task<bool> IsTimeSlotAvailableAsync(int barberProfileId, DateTime date, TimeSpan time, int durationMinutes)
        {
            // Check if barber is available on this day of week
            var dayOfWeek = date.DayOfWeek;
            var availability = await _context.BarberAvailabilities
                .FirstOrDefaultAsync(ba => ba.BarberProfileId == barberProfileId && 
                                          ba.DayOfWeek == dayOfWeek && 
                                          ba.IsAvailable);
            
            if (availability == null)
            {
                return false; // Barber doesn't work on this day
            }
            
            // Check if requested time is within working hours
            var timeEnd = time.Add(TimeSpan.FromMinutes(durationMinutes));
            if (time < availability.StartTime || timeEnd > availability.EndTime)
            {
                return false;
            }
            
            // Check if there's no overlapping appointment
            var appointmentStart = time;
            var appointmentEnd = time.Add(TimeSpan.FromMinutes(durationMinutes));
            
            var existingAppointments = await _context.Appointments
                .Where(a => a.BarberProfileId == barberProfileId && 
                           a.Date == date.Date && 
                           a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();
            
            foreach (var existingAppt in existingAppointments)
            {
                var existingStart = existingAppt.Time;
                var existingEnd = existingAppt.Time.Add(TimeSpan.FromMinutes(existingAppt.Service.DurationMinutes));
                
                // Check for overlap
                if (appointmentStart < existingEnd && appointmentEnd > existingStart)
                {
                    return false; // Time slot is already booked
                }
            }
            
            return true;
        }
        
        public async Task<List<DateTime>> GetAvailableDatesAsync(int barberProfileId, DateTime startDate, int daysAhead = 30)
        {
            var availableDates = new List<DateTime>();
            var endDate = startDate.AddDays(daysAhead);
            
            var availability = await _context.BarberAvailabilities
                .Where(ba => ba.BarberProfileId == barberProfileId && ba.IsAvailable)
                .ToListAsync();
            
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (availability.Any(a => a.DayOfWeek == date.DayOfWeek))
                {
                    availableDates.Add(date.Date);
                }
            }
            
            return availableDates;
        }
        
        public async Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(int barberProfileId, DateTime date, int durationMinutes)
        {
            var dayOfWeek = date.DayOfWeek;
            var availability = await _context.BarberAvailabilities
                .FirstOrDefaultAsync(ba => ba.BarberProfileId == barberProfileId && 
                                          ba.DayOfWeek == dayOfWeek && 
                                          ba.IsAvailable);
            
            if (availability == null)
            {
                return new List<TimeSpan>();
            }
            
            var availableSlots = new List<TimeSpan>();
            var currentTime = availability.StartTime;
            
            while (currentTime.Add(TimeSpan.FromMinutes(durationMinutes)) <= availability.EndTime)
            {
                bool isAvailable = await IsTimeSlotAvailableAsync(barberProfileId, date, currentTime, durationMinutes);
                if (isAvailable)
                {
                    availableSlots.Add(currentTime);
                }
                
                currentTime = currentTime.Add(TimeSpan.FromMinutes(15)); // 15-minute intervals
            }
            
            return availableSlots;
        }
    }
}

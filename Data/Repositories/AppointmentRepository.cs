using Garage88.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Garage88.Data.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllAppointmentsAtLaterDates()
        {
            return _context.Appointments.Include(a => a.Mechanic)
                                        .Include(a => a.Vehicle)
                                        .Include(a => a.CreatedBy)
                                        .Include(a => a.UpdatedBy)
                                        .Include(a => a.Client)
                                        .Include(a => a.Estimate)
                                        .Where(a => a.AppointmentStartDate >= DateTime.Today);
        }

        public string GetAllEvents()
        {
            var events = _context.Appointments
                .Include(a => a.Vehicle)
                .Include(a => a.Client)
                .Include(a => a.Mechanic)
                .Include(a => a.Estimate)
                .ThenInclude(e => e.Services)
                .Select(e => new
                {
                    id = e.Id,
                    title = e.Vehicle.PlateNumber + " : " + e.Client.FirstName + " " + e.Client.LastName + " Technician: " + e.Mechanic.FirstName + " " + e.Mechanic.LastName,
                    start = e.AppointmentStartDate.ToString(),
                    end = e.AppointmentEndDate.ToString(),
                    color = e.Mechanic.Color,
                    description = "<b>Vehicle: </b>" + e.Vehicle.PlateNumber + " (" + e.Vehicle.Brand.Name + " " + e.Vehicle.Model.Name + ")"
                + " <br> <b> Client:</b> " + e.Client.FirstName + " " + e.Client.LastName
                + " <br><b> Technician:</b> " + e.Mechanic.FirstName + " " + e.Mechanic.LastName
                + "<br><b>Services:</b><br>" + e.AppointmentServicesDetails
                + "<br><b>Observations:</b><br>" + e.Observations,
                    asAttended = e.AsAttended,
                }).ToList();

            return System.Text.Json.JsonSerializer.Serialize(events);

            //return new JsonResult(events);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int eventId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.CreatedBy)
                .Include(a => a.UpdatedBy)
                .Include(a => a.Client)
                .Include(a => a.Estimate)
                .Include(a => a.Mechanic)
                .Include(a => a.Vehicle)
                   .ThenInclude(v => v.Brand)
                .Include(a => a.Vehicle)
                   .ThenInclude(v => v.Model)
                .Where(a => a.Id == eventId).FirstOrDefaultAsync();

            return appointment;
        }

        public async Task<List<Appointment>> GetTommorowAppointmentsAsync(DateTime tommorowDate)
        {
            List<Appointment> appointments = await _context.Appointments
                                    .Include(a => a.CreatedBy)
                                    .Include(a => a.UpdatedBy)
                                    .Include(a => a.Client)
                                    .Include(a => a.Estimate)
                                    .Include(a => a.Mechanic)
                                    .Include(a => a.Vehicle)
                                       .ThenInclude(v => v.Brand)
                                    .Include(a => a.Vehicle)
                                       .ThenInclude(v => v.Model)
                                    .Where(a => a.AppointmentStartDate.Date == tommorowDate.Date).ToListAsync();

            return appointments;
        }
    }
}

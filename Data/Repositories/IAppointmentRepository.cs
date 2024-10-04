using Garage88.Data.Entities;

namespace Garage88.Data.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        IQueryable GetAllAppointmentsAtLaterDates();

        string GetAllEvents();

        Task<Appointment> GetAppointmentByIdAsync(int eventId);

        Task<List<Appointment>> GetTommorowAppointmentsAsync(DateTime tommorowDate);

        Task UpdateAsync(Appointment appointment);
    }
}
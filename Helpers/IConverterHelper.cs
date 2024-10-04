using Garage88.Data.Entities;
using Garage88.Models;

namespace Garage88.Helpers
{
    public interface IConverterHelper
    {
        Role toRole(RoleViewModel model, bool isNew);

        RoleViewModel toRoleViewModel(Role role);

        Task<Mechanic> ToMechanic(MechanicViewModel model, User user, bool isNew);

        MechanicViewModel ToMechanicViewModel(Mechanic mechanic, bool isNew);

        ServiceViewModel ToServiceViewModel(Service service);

        Service ToService(ServiceViewModel model, bool isNew);

        AppointmentViewModel ToAppointmentViewModel(Appointment appointment, bool isNew);

        Task<List<EstimateDetailTemp>> ToEstimateDetailTemps(IEnumerable<EstimateDetail> estimateDetails, string username);
    }
}
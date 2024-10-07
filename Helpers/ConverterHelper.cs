using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Models;

namespace Garage88.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly IMechanicsRolesRepository _mechanicsRolesRepository;
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IEstimateRepository _estimateRepository;

        public ConverterHelper(IUserHelper userHelper,
                                IMechanicsRolesRepository mechanicsRolesRepository,
                                IMechanicRepository mechanicRepository, 
                                IEstimateRepository estimateRepository)
        {
            _userHelper = userHelper;
            _mechanicsRolesRepository = mechanicsRolesRepository;
            _mechanicRepository = mechanicRepository;
            _estimateRepository = estimateRepository;
        }

        public AppointmentViewModel ToAppointmentViewModel(Appointment appointment, bool isNew)
        {
            var model = new AppointmentViewModel
            {
                AppointmentEndDate = appointment.AppointmentEndDate,
                AppointmentStartDate = appointment.AppointmentStartDate,
                AppointmentServicesDetails = appointment.AppointmentServicesDetails,
                CreatedBy = appointment.CreatedBy,
                CreatedDate = appointment.CreatedDate,
                Client = appointment.Client,
                ClientId = appointment.Client.Id,
<<<<<<< Updated upstream
                EmployeeId = appointment.Mechanic.Id,
=======
>>>>>>> Stashed changes
                Estimate = appointment.Estimate,
                EstimateId = appointment.Estimate.Id,
                Mechanic = appointment.Mechanic,
                MechanicId = appointment.Mechanic.Id,
                Observations = appointment.Observations,
                UpdatedBy = appointment.UpdatedBy,
                UpdatedDate = appointment.UpdatedDate,
                Vehicle = appointment.Vehicle,
                VehicleId = appointment.Vehicle.Id,
                Id = isNew ? 0 : appointment.Id,
                Technicians = _mechanicRepository.GetComboTechnicians()
            };

            return model;
        }

        public async Task<List<EstimateDetailTemp>> ToEstimateDetailTemps(IEnumerable<EstimateDetail> estimateDetails, string username)
        {
            List<EstimateDetailTemp> list = new List<EstimateDetailTemp>();

            foreach (EstimateDetail temp in estimateDetails)
            {
                var estimateDetailTemp = new EstimateDetailTemp
                {
                    ClientId = temp.ClientId,
                    Price = temp.Price,
                    Quantity = temp.Quantity,
                    Service = temp.Service,
                    User = await _userHelper.GetUserByEmailAsync(username),
                    VehicleId = temp.VehicleId,
                };

                list.Add(estimateDetailTemp);
            }

            return list;

        }

        public Role toRole(RoleViewModel model, bool isNew)
        {
            return new Role
            {
                Id = isNew ? 0 : model.RoleId,
                Name = model.Name,
                PermissionsName = model.SelectedPermission
            };
        }

        public RoleViewModel toRoleViewModel(Role role)
        {
            return new RoleViewModel
            {
                RoleId = role.Id,
                Name = role.Name,
                SelectedPermission = role.PermissionsName,
                Permissions = _userHelper.GetComboExistingRoles()
            };
        }

        public Service ToService(ServiceViewModel model, bool isNew)
        {
            return new Service
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                Description = model.Description,
                Discount = model.Discount,
                Price = model.Price
            };

        }

        public ServiceViewModel ToServiceViewModel(Service service)
        {
            return new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Discount = service.Discount,
                Price = service.Price,
            };
        }

        public async Task<Mechanic> ToMechanic(MechanicViewModel model, User user, bool isNew)
        {

            var role = await _mechanicsRolesRepository.GetRoleWithSpecialtiesAsync(model.RoleId);

            if (role == null)
            {
                return null;
            }

            var speciality = await _mechanicsRolesRepository.GetSpecialtyAsync(model.SpecialtyId);

            if (speciality == null)
            {
                return null;
            }

            var mechanic = new Mechanic
            {
                Id = isNew ? 0 : model.MechanicId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                About = model.About,
                Role = role,
                Specialty = speciality,
                User = user,
                Email = model.Email,
                Color = model.Color,
                PhotoId = model.PhotoId == Guid.Empty ? new Guid() : model.PhotoId,
            };

            if (isNew)
            {
                try
                {
                    await _mechanicRepository.CreateAsync(mechanic);
                    return mechanic;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return mechanic;
        }

        public MechanicViewModel ToMechanicViewModel(Mechanic mechanic, bool isNew)
        {
            var model = new MechanicViewModel
            {
                MechanicId = mechanic.Id,
                FirstName = mechanic.FirstName,
                LastName = mechanic.LastName,
                About = mechanic.About,
                Address = mechanic.User.Address,
                Email = mechanic.Email,
                PhoneNumber = mechanic.User.PhoneNumber,
                User = mechanic.User,
                RoleId = mechanic.Role.Id,
                SpecialtyId = mechanic.Specialty.Id,
                Roles = _mechanicsRolesRepository.GetComboRoles(),
                Specialities = _mechanicsRolesRepository.GetComboSpecialty(isNew ? 0 : mechanic.Role.Id),
                UserId = mechanic.User.Id,
                Color = mechanic.Color,
                PhotoId = mechanic.PhotoId
            };

            return model;
        }
    }
}

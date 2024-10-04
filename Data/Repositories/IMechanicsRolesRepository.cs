using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Data.Repositories
{
    public interface IMechanicsRolesRepository : IGenericRepository<Role>
    {
        IQueryable GetRolesWithSpecialties();

        Task<Role> GetRoleWithSpecialtiesAsync(int id);

        Task<Speciality> GetSpecialtyAsync(int id);

        Task AddSpecialtyAsync(SpecialityViewModel model);

        Task<int> UpdateSpecialtyAsync(Speciality model);

        Task<int> DeleteSpecialtyAsync(Speciality model);

        IEnumerable<SelectListItem> GetComboRoles();

        IEnumerable<SelectListItem> GetComboSpecialty(int roleId);

        Task<Role> GetRoleAsync(Speciality model);

        Task<int> GetRoleIdWithSpecialtyAsync(int specialtyId);

    }
}

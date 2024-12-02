using Garage88.Data.Entities;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace Garage88.Data.Repositories
{
    public interface IMechanicsRolesRepository : IGenericRepository<Role>
    {
        IQueryable GetRolesWithSpecialities();

        Task<Role> GetRoleWithSpecialitiesAsync(int id);

        Task<Speciality> GetSpecialityAsync(int id);

        Task AddSpecialityAsync(SpecialityViewModel model);

        Task<int> UpdateSpecialityAsync(Speciality model);

        Task<int> DeleteSpecialityAsync(Speciality model);

        IEnumerable<SelectListItem> GetComboRoles();

        IEnumerable<SelectListItem> GetComboSpeciality(int roleId);

        Task<Role> GetRoleAsync(Speciality model);

        Task<int> GetRoleIdWithSpecialityAsync(int specialityId);

    }
}

using Garage88.Data.Entities;
using Garage88.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace Garage88.Data.Repositories
{
    public interface IMechanicRepository : IGenericRepository<Mechanic>
    {
        IQueryable GetAllWithUsers();

        Task<Mechanic> GetEmployeeByIdAsync(int employeeId);

        Task<Response> CheckIfEmployeeExistsAsync(User user);

        Task<Mechanic> GetByEmailAsync(string email);

        IEnumerable<SelectListItem> GetComboTechnicians();
        Task<List<Mechanic>> GetTechniciansEmployeesAsync();
        Task AddAsync(Mechanic mechanic);
        Task<IEnumerable> GetAllAsync();
    }
}
using Garage88.Data.Entities;
using Garage88.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Data.Repositories
{
    public interface IMechanicRepository : IGenericRepository<Mechanic>
    {
        IQueryable GetAllWithUsers();

        Task<Mechanic> GetMechanicByIdAsync(int mechanicId);

        Task<Response> CheckIfMechanicExistsAsync(User user);

        Task<Mechanic> GetByEmailAsync(string email);

        IEnumerable<SelectListItem> GetComboTechnicians();

        Task<List<Mechanic>> GetTechniciansMechanicsAsync();
    }
}
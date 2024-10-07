using Garage88.Data.Entities;
using Garage88.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Data.Repositories
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        IQueryable GetAllWithUsers();

        Task<Client> GetClientWithUserByIdAsync(int clientId);

        Task<bool> CheckIfClientInBdByEmailAsync(string clientEmail);

        Task<Client> GetClientByUserIdAsync(string userId);

        Task<Client> GetClientByEmailAsync(string email);

        IEnumerable<SelectListItem> GetComboClient();

        Task<List<Vehicle>> GetClientVehicleAsync(int clientId);
    }
}
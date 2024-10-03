using Garage88.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Data.Repositories
{
    public interface IClientRepository
    {
        IQueryable GetAllWithUsers();

        Task<Client> GetClientWithUserByIdAsync(int clientId);

        Task<bool> CheckIfClientInBdByEmailAsync(string clientEmail);

        Task<Client> GetClientByUserIdAsync(string userId);

        Task<Client> GetClientByEmailAsync(string email);

        IEnumerable<SelectListItem> GetComboClients();

        Task<List<Vehicle>> GetClientVehicleAsync(int clientId);

        Task AddAsync(Client client);

        Task<string?> GetAllAsync();
    }
}
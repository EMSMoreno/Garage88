using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage88.Data.Entities;

namespace Garage88.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public Task AddAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckIfClientInBdByEmailAsync(string clientEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckIfCustomerInBdByEmailAsync(string customerEmail)
        {
            bool inBd = false;

            var customer = await _context.Clients.Where(ctm => ctm.Email == customerEmail).FirstOrDefaultAsync();

            if (customer != null)
            {
                inBd = true;
            }

            return inBd;
        }

        public Task<string?> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IQueryable GetAllWithUsers()
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClientByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClientByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Vehicle>> GetClientVehicleAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public Task<Client> GetClientWithUserByIdAsync(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SelectListItem> GetComboClients()
        {
            throw new NotImplementedException();
        }
    }
}

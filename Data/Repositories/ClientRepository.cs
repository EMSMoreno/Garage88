using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage88.Data.Entities;
using Garage88.Helpers;
using Garage88.Data.Repositories;

namespace Garage88.Data.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckIfClientInBdByEmailAsync(string customerEmail)
        {
            bool inBd = false;

            var customer = await _context.Clients.Where(ctm => ctm.Email == customerEmail).FirstOrDefaultAsync();

            if (customer != null)
            {
                inBd = true;
            }

            return inBd;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Clients.Include(u => u.User)
                                     .Include(v => v.Vehicles)
                                     .OrderBy(o => o.FirstName);
        }

        public IEnumerable<SelectListItem> GetComboClient()
        {

            var list = _context.Clients.Select(b => new SelectListItem
            {
                Text = $"{b.FirstName} {b.LastName}",
                Value = b.Id.ToString()
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Client]",
                Value = "0"
            });

            return list;
        }

        public async Task<Client> GetClientByEmailAsync(string email)
        {
            return await _context.Clients.Include(u => u.User)
                                           .Include(v => v.Vehicles)
                                           .Where(c => c.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Client> GetClientByUserIdAsync(string userId)
        {
            return await _context.Clients.Include(v => v.Vehicles).Where(c => c.User.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<Client> GetClientWithUserByIdAsync(int clientId)
        {
            var customer = await _context.Clients.Include(u => u.User)
                                                   .Include(v => v.Vehicles)
                                                   .ThenInclude(v => v.Brand)
                                                   .ThenInclude(b => b.Models)
                                                   .FirstOrDefaultAsync(u => u.Id == clientId);

            return customer;
            ;
        }

        public async Task<List<Vehicle>> GetClientVehicleAsync(int clientId)
        {
            var clientVehicles = await _context.Vehicles.Where(p => p.ClientId == clientId).ToListAsync();

            return clientVehicles;


        }
    }
}

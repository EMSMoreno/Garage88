using Garage88.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace Garage88.Data.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly DataContext _context;

        public VehicleRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public Task AddAsync(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAllRegisteredVehiclesNumberAsync()
        {
            throw new NotImplementedException();
        }

        public IQueryable GetAllWithCustomers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SelectListItem> GetComboVehicles(int customerId)
        {
            throw new NotImplementedException();
        }

        public IQueryable GetCustomerVehiclesAsync(int customerId)
        {
            throw new NotImplementedException();
        }

        public Task<Vehicle> GetNewlyAddedVehicleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Vehicle> GetVehicleDetailsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

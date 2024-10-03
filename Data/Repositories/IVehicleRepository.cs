using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace Garage88.Data.Repositories
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        IQueryable GetAllWithCustomers();

        IEnumerable<SelectListItem> GetComboVehicles(int customerId);

        Task<Vehicle> GetVehicleDetailsByIdAsync(int id);

        IQueryable GetCustomerVehiclesAsync(int customerId);

        Task<Vehicle> GetNewlyAddedVehicleAsync(int id);

        Task<int> GetAllRegisteredVehiclesNumberAsync();
        Task AddAsync(Vehicle vehicle);
        Task<IEnumerable> GetAllAsync();
    }
}
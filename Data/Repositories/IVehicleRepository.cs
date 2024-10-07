using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;

namespace Garage88.Data.Repositories
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        IQueryable GetAllWithClients();

        IEnumerable<SelectListItem> GetComboVehicles(int clientId);

        Task<Vehicle> GetVehicleDetailsByIdAsync(int id);

        IQueryable GetClientVehiclesAsync(int clientId);

        Task<Vehicle> GetNewlyAddedVehicleAsync(int id);

        Task<int> GetAllRegisteredVehiclesNumberAsync();

        Task<List<VehicleChartModel>> GetVehiclesChartDataAsync();
    }
}
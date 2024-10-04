using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Data.Repositories
{
    public interface IServiceRepository : IGenericRepository<Service>
    {
        public IEnumerable<SelectListItem> GetComboServices();

        Task AddServiceToEstimateAsync(AddServiceToEstimateViewModel model, string userName);

        Task<List<ServiceChartModel>> GetMostSoldServicesData();
    }
}

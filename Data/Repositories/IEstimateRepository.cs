using Garage88.Data.Entities;
using Garage88.Helpers;

namespace Garage88.Data.Repositories
{
    public interface IEstimateRepository : IGenericRepository<Estimate>
    {
        IQueryable GetAllEstimates();

        Task<IQueryable<EstimateDetailTemp>> GetDetailTempsAsync(int vehicleId, int clientId);

        Task CreateEstimateDetailTemp(EstimateDetailTemp estimateDetailTemp);

        Task<EstimateDetailTemp> GetEstimateDetailTempAsync(string userName);

        Task ModifyEstimateDetailTempQuantityAsync(int id, double quantity);

        Task DeleteDetailTempAsync(int id);

        Task<Response> ConfirmEstimateAsync(string username, int clientId, int vehicleId, string faultdescription);

        Task<EstimateDetailTemp> GetEstimateDetailTempWithVehicleIdAsync(string name, Vehicle vehicle);

        Task<EstimateDetailTemp> GetEstimateDetailTempByIdAsync(int id);

        Task<Estimate> GetEstimateWithDetailsByIdAsync(int value);

        Task<int> DeleteEstimateDetailsAsync(int id);

        Task CreateEstimatesDetailsTemps(IEnumerable<EstimateDetailTemp> estimateDetailTemps);

        Task<int> DeleteEstimateDetailTempsAsync(int vehicleId, int clientId);

        Task<Response> UpdateEstimateAsync(string username, int clientId, int vehicleId, string faultdescription);

        Task<Estimate> GetCreatedEstimateAsync(string userId, int clientId, int vehicleId);
    }
}
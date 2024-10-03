using Garage88.Data.Entities;
using Garage88.Helpers;

namespace Garage88.Data.Repositories
{
    public interface IBillingRepository
    {
        IQueryable GetAllEstimates();

        Task ModifyEstimateDetailTempQuantityAsync(int id, double quantity);

        Task DeleteDetailTempAsync(int id);

        Task<Response> ConfirmEstimateAsync(string username, int customerId, int vehicleId, string faultdescription);
        
        Task<int> DeleteEstimateDetailsAsync(int id);

        Task<int> DeleteEstimateDetailTempsAsync(int vehicleId, int customerId);

        Task<Response> UpdateEstimateAsync(string username, int customerId, int vehicleId, string faultdescription);
        Task AddAsync(Billing billing);
    }
}
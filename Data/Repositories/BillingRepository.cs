using Microsoft.EntityFrameworkCore;
using Garage88.Data.Entities;
using Garage88.Helpers;

namespace Garage88.Data.Repositories
{
    public class BillingRepository : GenericRepository<Billing>, IBillingRepository
    {

        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _customerRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public BillingRepository(DataContext context, IUserHelper userHelper,
            IClientRepository customerRepository, IVehicleRepository vehicleRepository) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
            _customerRepository = customerRepository;
            _vehicleRepository = vehicleRepository;
        }

        public Task AddAsync(Billing billing)
        {
            throw new NotImplementedException();
        }

        public Task<Response> ConfirmEstimateAsync(string username, int customerId, int vehicleId, string faultdescription)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDetailTempAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteEstimateDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteEstimateDetailTempsAsync(int vehicleId, int customerId)
        {
            throw new NotImplementedException();
        }

        public IQueryable GetAllEstimates()
        {
            throw new NotImplementedException();
        }

        public Task ModifyEstimateDetailTempQuantityAsync(int id, double quantity)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateEstimateAsync(string username, int customerId, int vehicleId, string faultdescription)
        {
            throw new NotImplementedException();
        }
    }
}

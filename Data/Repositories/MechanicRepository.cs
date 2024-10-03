using Microsoft.AspNetCore.Mvc.Rendering;
using Garage88.Data.Entities;
using Garage88.Helpers;
using System.Collections;

namespace Garage88.Data.Repositories
{
    public class MechanicRepository : GenericRepository<Mechanic>, IMechanicRepository
    {
        private readonly DataContext _context;

        public MechanicRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public Task AddAsync(Mechanic mechanic)
        {
            throw new NotImplementedException();
        }

        public Task<Response> CheckIfEmployeeExistsAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IQueryable GetAllWithUsers()
        {
            throw new NotImplementedException();
        }

        public Task<Mechanic> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SelectListItem> GetComboTechnicians()
        {
            throw new NotImplementedException();
        }

        public Task<Mechanic> GetEmployeeByIdAsync(int employeeId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Mechanic>> GetTechniciansEmployeesAsync()
        {
            throw new NotImplementedException();
        }
    }
}

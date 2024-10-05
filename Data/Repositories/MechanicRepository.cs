using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage88.Data.Entities;
using Garage88.Helpers;

namespace Garage88.Data.Repositories
{
    public class MechanicRepository : GenericRepository<Mechanic>, IMechanicRepository
    {
        private readonly DataContext _context;

        public MechanicRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Response> CheckIfEmployeeExistsAsync(User user)
        {
            var employee = await _context.Mechanics.FindAsync(user);

            if (employee == null)
            {
                return new Response { IsSuccess = false };
            }
            else
            {
                return new Response { IsSuccess = true };
            }
        }

        public Task<Response> CheckIfMechanicExistsAsync(User user)
        {
            throw new NotImplementedException();
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Mechanics.Include(e => e.User)
                                     .Include(e => e.Role)
                                     .Include(e => e.Speciality);
        }

        public async Task<Mechanic> GetByEmailAsync(string email)
        {
            var employee = await _context.Mechanics.Include(e => e.User)
                                                   .Include(e => e.Role)
                                                   .Include(e => e.Speciality)
                                                   .Where(e => e.Email == email).FirstOrDefaultAsync();

            return employee;
        }

        public IEnumerable<SelectListItem> GetComboTechnicians()
        {
            var list = _context.Mechanics.Include(e => e.Role)
                                         .Where(e => e.Role.PermissionsName == "Technician")
                                         .Select(e => new SelectListItem
                                         {
                                             Text = e.FirstName + " " + e.LastName,
                                             Value = e.Id.ToString(),
                                         }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a Technician]",
                Value = "0"
            });

            return list;
        }

        public async Task<Mechanic> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Mechanics.Include(e => e.User)
                                                   .Include(e => e.Role)
                                                   .Include(e => e.Speciality)
                                                   .Where(m => m.Id == employeeId).FirstOrDefaultAsync();

            return employee;
        }

        public Task<Mechanic> GetMechanicByIdAsync(int mechanicId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Mechanic>> GetTechniciansEmployeesAsync()
        {
            List<Mechanic> list = new List<Mechanic>();

            list = await _context.Mechanics.Include(e => e.User).Include(e => e.Role).Include(e => e.Speciality).Where(e => e.Role.Name == "Technician").ToListAsync();

            return list;
        }

        public Task<List<Mechanic>> GetTechniciansMechanicsAsync()
        {
            throw new NotImplementedException();
        }
    }
}

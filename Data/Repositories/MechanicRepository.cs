using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage88.Data.Entities;
using Garage88.Helpers;
using System.Data.SqlTypes;

namespace Garage88.Data.Repositories
{
    public class MechanicRepository : GenericRepository<Mechanic>, IMechanicRepository
    {
        private readonly DataContext _context;

        public MechanicRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Response> CheckIfMechanicExistsAsync(User user)
        {
            var mechanic = await _context.Mechanics.FirstOrDefaultAsync(m => m.User.Id == user.Id);

            if (mechanic == null)
            {
                return new Response { IsSuccess = false };
            }
            return new Response { IsSuccess = true };
        }

        public IQueryable GetAllWithUsers()
        {
            return GetMechanicsWithIncludes();
        }

        public async Task<Mechanic> GetByEmailAsync(string email)
        {
            return await GetMechanicsWithIncludes().FirstOrDefaultAsync(e => e.Email == email);
        }

        public IEnumerable<SelectListItem> GetComboTechnicians()
        {
            var list = _context.Mechanics.Include(e => e.Role)
                                         .Where(e => e.Role.PermissionsName == "Mechanic")
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

        public async Task<Mechanic> GetMechanicByIdAsync(int mechanicId)
        {
            return await GetMechanicsWithIncludes().FirstOrDefaultAsync(m => m.Id == mechanicId);
        }

        public async Task<List<Mechanic>> GetTechniciansMechanicsAsync()
        {
            return await _context.Mechanics
                    .Include(m => m.Role)
                    .Include(m => m.Speciality)
                    .ToListAsync();
        }

        private IQueryable<Mechanic> GetMechanicsWithIncludes()
        {
            return _context.Mechanics.Include(e => e.User)
                                     .Include(e => e.Role)
                                     .Include(e => e.Speciality);
        }

    }
}

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
            var mechanics = await _context.Mechanics.FindAsync(user);

            if (mechanics == null)
            {
                return new Response { IsSuccess = false };
            }
            else
            {
                return new Response { IsSuccess = true };
            }
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Mechanics.Include(e => e.User)
                                     .Include(e => e.Role)
                                     .Include(e => e.Speciality);
        }

        public async Task<Mechanic> GetByEmailAsync(string email)
        {
            var mechanic = await _context.Mechanics.Include(e => e.User)
                                                   .Include(e => e.Role)
                                                   .Include(e => e.Speciality)
                                                   .Where(e => e.Email == email).FirstOrDefaultAsync();

            return mechanic;
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
            var mechanic = await _context.Mechanics.Include(e => e.User)
                                                   .Include(e => e.Role)
                                                   .Include(e => e.Speciality)
                                                   .Where(m => m.Id == mechanicId).FirstOrDefaultAsync();

            return mechanic;
        }

        public async Task<List<Mechanic>> GetTechniciansMechanicsAsync()
        {
            List<Mechanic> list = new List<Mechanic>();

            list = await _context.Mechanics.Include(e => e.User).Include(e => e.Role).Include(e => e.Speciality).Where(e => e.Role.Name == "Technician").ToListAsync();

            return list;
        }

    }
}

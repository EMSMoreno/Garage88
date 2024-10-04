using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Garage88.Data.Repositories
{
    public class MechanicsRolesRepository : GenericRepository<Role>, IMechanicsRolesRepository
    {
        private readonly DataContext _context;

        public MechanicsRolesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddSpecialtyAsync(SpecialityViewModel model)
        {
            var role = await this.GetRoleWithSpecialtiesAsync(model.RoleId);

            if (role == null)
            {
                return;
            }

            role.Specialties.Add(new Speciality
            {
                Name = model.Name,
            });

            _context.MechanicsRoles.Update(role);

            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteSpecialtyAsync(Speciality model)
        {
            var role = await _context.MechanicsRoles.Where(er => er.Specialties.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }

            _context.Specialties.Remove(model);

            await _context.SaveChangesAsync();

            return role.Id;
        }

        public IEnumerable<SelectListItem> GetComboRoles()
        {
            var list = _context.MechanicsRoles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Insert the Role]",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboSpecialty(int roleId)
        {
            var role = _context.MechanicsRoles.Find(roleId);
            var list = new List<SelectListItem>();

            if (role != null)
            {
                list = _context.Specialties.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "[Select a Specialty]",
                    Value = "0"
                });
            }
            return list;
        }

        public async Task<Role> GetRoleAsync(Speciality model)
        {
            return await _context.MechanicsRoles.Where(r => r.Specialties.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();
        }

        public async Task<int> GetRoleIdWithSpecialtyAsync(int specialtyId)
        {
            var role = await _context.MechanicsRoles.Where(r => r.Specialties.Any(s => s.Id == specialtyId)).FirstOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }
            else
            {
                return role.Id;
            }
        }

        public IQueryable GetRolesWithSpecialties()
        {
            return _context.MechanicsRoles.Include(r => r.Specialties).OrderBy(s => s.Name);
        }

        public async Task<Role> GetRoleWithSpecialtiesAsync(int id)
        {
            return await _context.MechanicsRoles.Include(r => r.Specialties).Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Speciality> GetSpecialtyAsync(int id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task<int> UpdateSpecialtyAsync(Speciality model)
        {
            var role = await _context.MechanicsRoles.Where(r => r.Specialties.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }

            _context.Specialties.Update(model);
            await _context.SaveChangesAsync();
            return role.Id;
        }
    }
}

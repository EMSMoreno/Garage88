using Garage88.Data.Entities;
using Garage88.Helpers;
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

        public async Task AddSpecialityAsync(SpecialityViewModel model)
        {
            var role = await this.GetRoleWithSpecialitiesAsync(model.RoleId);

            if (role == null)
            {
                return;
            }

            role.Specialities.Add(new Speciality
            {
                Name = model.Name,
            });

            _context.MechanicsRoles.Update(role);

            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteSpecialityAsync(Speciality model)
        {
            var role = await _context.MechanicsRoles.Where(er => er.Specialities.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }

            _context.Specialities.Remove(model);

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

        public IEnumerable<SelectListItem> GetComboSpeciality(int roleId)
        {
            var role = _context.MechanicsRoles.Find(roleId);
            var list = new List<SelectListItem>();

            if (role != null)
            {
                list = _context.Specialities.Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "[Select a Speciality]",
                    Value = "0"
                });
            }

            return list;
        }

        public async Task<Role> GetRoleAsync(Speciality model)
        {
            return await _context.MechanicsRoles.Where(r => r.Specialities.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();
        }

        public async Task<int> GetRoleIdWithSpecialityAsync(int specialityId)
        {
            var role = await _context.MechanicsRoles.Where(r => r.Specialities.Any(s => s.Id == specialityId)).FirstOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }
            else
            {
                return role.Id;
            }
        }

        public IQueryable GetRolesWithSpecialities()
        {
            return _context.MechanicsRoles.Include(r => r.Specialities).OrderBy(s => s.Name);
        }

        public async Task<Role> GetRoleWithSpecialitiesAsync(int id)
        {
            return await _context.MechanicsRoles.Include(r => r.Specialities).Where(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Speciality> GetSpecialityAsync(int id)
        {
            return await _context.Specialities.FindAsync(id);
        }

        public async Task<int> UpdateSpecialityAsync(Speciality model)
        {
            var role = await _context.MechanicsRoles.Where(r => r.Specialities.Any(s => s.Id == model.Id)).FirstOrDefaultAsync();

            if (role == null)
            {
                return 0;
            }

            _context.Specialities.Update(model);
            await _context.SaveChangesAsync();
            return role.Id;
        }
    }
}

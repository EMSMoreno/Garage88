using Garage88.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garage88.Data.Repositories
{
    public class RepairRepository : IRepairRepository
    {
        private readonly DataContext _context;

        public RepairRepository(DataContext context)
        {
            _context = context;
        }

        // Listar todas as reparações
        public async Task<IEnumerable<Repair>> GetAllRepairsAsync()
        {
            return await _context.Repairs.ToListAsync();
        }

        // Obter uma reparação pelo ID
        public async Task<Repair> GetRepairByIdAsync(int id)
        {
            return await _context.Repairs.FindAsync(id);
        }

        // Adicionar nova reparação
        public async Task AddRepairAsync(Repair repair)
        {
            await _context.Repairs.AddAsync(repair);
            await _context.SaveChangesAsync();
        }

        // Atualizar uma reparação existente
        public async Task UpdateRepairAsync(Repair repair)
        {
            _context.Repairs.Update(repair);
            await _context.SaveChangesAsync();
        }

        // Apagar uma reparação pelo ID
        public async Task DeleteRepairAsync(int id)
        {
            var repair = await _context.Repairs.FindAsync(id);
            if (repair != null)
            {
                _context.Repairs.Remove(repair);
                await _context.SaveChangesAsync();
            }
        }

        public Task AddAsync(Repair repair)
        {
            throw new NotImplementedException();
        }
    }
}

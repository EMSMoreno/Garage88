using Garage88.Data.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garage88.Data.Repositories
{
    public interface IRepairRepository
    {
        Task<IEnumerable<Repair>> GetAllRepairsAsync();    // Listar todas as reparações
        Task<Repair> GetRepairByIdAsync(int id);           // Obter uma reparação pelo ID
        Task AddRepairAsync(Repair repair);                // Adicionar nova reparação
        Task UpdateRepairAsync(Repair repair);             // Atualizar uma reparação existente
        Task DeleteRepairAsync(int id);                    // Apagar uma reparação pelo ID
        Task AddAsync(Repair repair);
        Task<IEnumerable> GetAllAsync();
    }
}

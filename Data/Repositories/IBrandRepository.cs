using Microsoft.AspNetCore.Mvc.Rendering;
using Garage88.Data.Entities;
using Garage88.Models;

namespace Garage88.Data.Repositories
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        IQueryable GetBrandsWithModels();

        Task<Brand> GetBrandWithModelsAsync(int id);

        Task<Model> GetModelAsync(int id);

        Task AddModelAsync(ModelViewModel model);

        Task<int> UpdateModelAsync(Model model);

        Task<int> DeleteModelAsync(Model model);

        IEnumerable<SelectListItem> GetComboBrands();

        IEnumerable<SelectListItem> GetComboModels(int brandId);

        Task<Brand> GetBrandAsync(Model model);

        Task<int> GetBrandIdWithVehicleModelAsync(int modelId);
    }
}
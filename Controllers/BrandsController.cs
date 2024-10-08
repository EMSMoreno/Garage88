using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Repositories;
using Garage88.Models;
using Vereyon.Web;
using Garage88.Data.Entities;

namespace Garage88.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class BrandsController : Controller
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IFlashMessage _flashMessage;

        public BrandsController(IBrandRepository brandRepository, IFlashMessage flashMessage)
        {
            _brandRepository = brandRepository;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            return View(_brandRepository.GetBrandsWithModels());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _brandRepository.CreateAsync(brand);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(brand);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandRepository.GetBrandWithModelsAsync(id.Value);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandRepository.GetBrandWithModelsAsync(id.Value);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            if (ModelState.IsValid)
            {
                await _brandRepository.UpdateAsync(brand);
                return RedirectToAction(nameof(Index));
            }

            return View(brand);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandRepository.GetBrandWithModelsAsync(id.Value);

            try
            {
                await _brandRepository.DeleteAsync(brand);
            }
            catch (Exception)
            {
                _flashMessage.Danger("There was an error deleting the brand, probably the brand is in use. Try deleting or changing vehicles with the specified brand.");
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddModel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _brandRepository.GetByIdAsync(id.Value);

            if (brand == null)
            {
                return NotFound();
            }

            var model = new ModelViewModel
            {
                BrandId = brand.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddModel(ModelViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _brandRepository.AddModelAsync(model);
                return RedirectToAction("Details", new { Id = model.BrandId });
            }

            return View(model);
        }

        public async Task<IActionResult> EditModel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _brandRepository.GetModelAsync(id.Value);

            if (model == null)
            {
                return NotFound();
            }

            var brandId = await _brandRepository.GetBrandIdWithVehicleModelAsync(model.Id);

            if (brandId == 0)
            {
                return NotFound();
            }

            var editableModel = new ModelViewModel
            {
                BrandId = brandId,
                ModelId = model.Id,
                Name = model.Name,
            };


            return View(editableModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditModel(ModelViewModel model)
        {
            if (ModelState.IsValid)
            {

                var modelToChange = await _brandRepository.GetModelAsync(model.ModelId);

                if (modelToChange == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error updating the model.");
                    return View(model);
                }

                modelToChange.Name = model.Name;

                try
                {
                    await _brandRepository.UpdateModelAsync(modelToChange);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }

                return RedirectToAction($"Details", new { id = model.BrandId });

            }
            return View(model);
        }

        public async Task<IActionResult> DeleteModel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _brandRepository.GetModelAsync(id.Value);

            if (model == null)
            {
                return NotFound();
            }

            var brandId = await _brandRepository.GetBrandIdWithVehicleModelAsync(model.Id);

            if (brandId == 0)
            {
                return NotFound();
            }

            try
            {
                await _brandRepository.DeleteModelAsync(model);
                return RedirectToAction($"Details", new { id = brandId });
            }
            catch (Exception)
            {
                _flashMessage.Danger("There was an error trying to delete the model. Probably the model is being used in a car. Try deleting cars with the model you want to delete first.");
            }

            return RedirectToAction($"Details", new { id = brandId });
        }
    }
}

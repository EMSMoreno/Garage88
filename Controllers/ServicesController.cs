using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Vereyon.Web;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IConverterHelper _converterHelper;

        public ServicesController(IServiceRepository serviceRepository, IFlashMessage flashMessage, IConverterHelper converterHelper)
        {
            _serviceRepository = serviceRepository;
            _flashMessage = flashMessage;
            _converterHelper = converterHelper;
        }

        public IActionResult Index()
        {
            return View(_serviceRepository.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Discount = model.Discount,
                };

                try
                {
                    await _serviceRepository.CreateAsync(service);
                    _flashMessage.Confirmation("The service was created with success.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was a problem creating the service. " + ex.InnerException.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);

            if (service == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToServiceViewModel(service);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = _converterHelper.ToService(model, false);

                if (service == null)
                {
                    _flashMessage.Danger("There was an error updating the service.");
                    return View(model);
                }

                try
                {
                    await _serviceRepository.UpdateAsync(service);
                    _flashMessage.Confirmation("The service was updated with success.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    _flashMessage.Danger("There was an error updating the service.");
                    return View(model);
                }

            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id.Value);

            if (service == null)
            {
                return NotFound();
            }

            try
            {
                await _serviceRepository.DeleteAsync(service);
                _flashMessage.Confirmation("The service was deleted with success.");
            }
            catch (Exception)
            {
                _flashMessage.Danger("There was an error deleting the service. Probably it is being used in some sort of entry. Please make sure the service is not being used in any file before trying to delete it.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("Services/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        //[HttpPost]
        //[Route("Services/ServiceDetails")]
        //public async Task<JsonResult> ServiceDetails(int Id)
        //{
        //    if (Id == 0)
        //    {
        //        return null;
        //    }

        //    var service = await _serviceRepository.GetByIdAsync(Id);

        //    var json = Json(service);

        //    return json;
        //}
    }
}

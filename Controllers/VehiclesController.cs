using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Vereyon.Web;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin, Technician, Receptionist")]
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IBrandRepository _brandRepository;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IFlashMessage _flashMessage;

        public VehiclesController(IVehicleRepository vehicleRepository,
            IClientRepository clientRepository,
            IUserHelper userHelper,
            IBrandRepository brandRepository,
            IEstimateRepository estimateRepository,
            IFlashMessage flashMessage)
        {
            _vehicleRepository = vehicleRepository;
            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _brandRepository = brandRepository;
            _estimateRepository = estimateRepository;
            _flashMessage = flashMessage;
        }

        // GET: Vehicles
        public IActionResult Index()
        {
            var vehicles = _vehicleRepository.GetAllWithClients();

            return View(vehicles);
        }

        // GET: Vehicles/Create
        public async Task<IActionResult> Create(string email, bool isEstimate)
        {
            var client = await _clientRepository.GetClientByEmailAsync(email);

            if (client == null)
            {
                return NotFound();
            }

            var vehicleModel = new VehicleViewModel
            {
                Client = client,
                ClientId = client.Id,
                Brands = _brandRepository.GetComboBrands(),
                Models = _brandRepository.GetComboModels(0),
                IsEstimate = isEstimate
            };

            return View(vehicleModel);
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VehicleViewModel vehicleModel)
        {
            var client = await _clientRepository.GetByIdAsync(vehicleModel.ClientId);

            if (client == null)
            {
                return NotFound();
            }

            vehicleModel.Client = client;

            if (ModelState.IsValid)
            {

                if (!CheckIfVinIsNull(vehicleModel.VehicleIdentificationNumber))
                {
                    if (vehicleModel.VehicleIdentificationNumber.Length < 17)
                    {
                        ModelState.AddModelError(string.Empty, "The Vehicle Identification Number (VIN) must have 17 characters.");
                        vehicleModel.Brands = _brandRepository.GetComboBrands();
                        vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                        return View(vehicleModel);
                    }
                }

                var brand = await _brandRepository.GetBrandWithModelsAsync(vehicleModel.BrandId);
                var model = await _brandRepository.GetModelAsync(vehicleModel.ModelId);

                if (model == null || brand == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error creating the vehicle.");
                    vehicleModel.Brands = _brandRepository.GetComboBrands();
                    vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                    return View(vehicleModel);
                }

                var vehicle = new Vehicle
                {
                    Brand = brand,
                    Model = model,
                    DateOfConstruction = vehicleModel.DateOfConstruction,
                    PlateNumber = vehicleModel.PlateNumber,
                    VehicleIdentificationNumber = vehicleModel.VehicleIdentificationNumber,
                    Horsepower = vehicleModel.Horsepower,
                    ClientId = client.Id,
                };

                try
                {
                    await _vehicleRepository.CreateAsync(vehicle);

                    if (!vehicleModel.IsEstimate)
                    {
                        return RedirectToAction("Edit", "Client", new { id = vehicle.ClientId });
                    }
                    else
                    {
                        var addedVehicle = await _vehicleRepository.GetNewlyAddedVehicleAsync(client.Id);

                        var estimateDetailTemp = new EstimateDetailTemp
                        {
                            ClientId = client.Id,
                            VehicleId = addedVehicle.Id,
                            User = await _userHelper.GetUserByEmailAsync(User.Identity.Name)
                        };

                        try
                        {
                            await _estimateRepository.CreateEstimateDetailTemp(estimateDetailTemp);
                            return RedirectToAction("Create", "Estimates", new { id = estimateDetailTemp.VehicleId });
                        }
                        catch (Exception ex)
                        {
                            _flashMessage.Danger("There was an error creating the estimate. " + ex.InnerException.Message);
                            return RedirectToAction("Index", "Client");
                        }
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                }

            }

            vehicleModel.Brands = _brandRepository.GetComboBrands();
            vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
            return View(vehicleModel);
        }

        //// GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(id.Value);

            if (vehicle == null)
            {
                return NotFound();
            }

            var vehicleModel = new VehicleViewModel
            {
                BrandId = vehicle.Brand.Id,
                DateOfConstruction = vehicle.DateOfConstruction,
                ModelId = vehicle.Model.Id,
                VehicleId = vehicle.Id,
                ClientId = vehicle.ClientId,
                PlateNumber = vehicle.PlateNumber,
                VehicleIdentificationNumber = vehicle.VehicleIdentificationNumber,
                Horsepower = vehicle.Horsepower,
                Brands = _brandRepository.GetComboBrands(),
                Models = _brandRepository.GetComboModels(vehicle.Brand.Id)
            };

            return View(vehicleModel);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VehicleViewModel vehicleModel)
        {
            if (ModelState.IsValid)
            {
                if (!CheckIfVinIsNull(vehicleModel.VehicleIdentificationNumber))
                {
                    if (vehicleModel.VehicleIdentificationNumber.Length < 17)
                    {
                        ModelState.AddModelError(string.Empty, "The Vehicle Identification Number (VIN) must have 17 characters.");
                        vehicleModel.Brands = _brandRepository.GetComboBrands();
                        vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                        return View(vehicleModel);
                    }
                }

                var vehicle = await _vehicleRepository.GetByIdAsync(vehicleModel.VehicleId);
                var brand = await _brandRepository.GetByIdAsync(vehicleModel.BrandId);
                var model = await _brandRepository.GetModelAsync(vehicleModel.ModelId);

                if (model == null || brand == null || vehicle == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error editing the vehicle.");
                    vehicleModel.Brands = _brandRepository.GetComboBrands();
                    vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                    return View(vehicleModel);
                }

                vehicle.PlateNumber = vehicleModel.PlateNumber;
                vehicle.VehicleIdentificationNumber = vehicleModel.VehicleIdentificationNumber;
                vehicle.Horsepower = vehicleModel.Horsepower;
                vehicle.Brand = brand;
                vehicle.ClientId = vehicleModel.ClientId;
                vehicle.Model = model;
                vehicle.DateOfConstruction = vehicleModel.DateOfConstruction;

                try
                {
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    vehicleModel.Brands = _brandRepository.GetComboBrands();
                    vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
                    return View(vehicleModel);
                }

                return RedirectToAction(nameof(Index));
            }
            vehicleModel.Brands = _brandRepository.GetComboBrands();
            vehicleModel.Models = _brandRepository.GetComboModels(vehicleModel.BrandId);
            return View(vehicleModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepository.GetByIdAsync(id.Value);

            if (vehicle == null)
            {
                return NotFound();
            }

            try
            {
                await _vehicleRepository.DeleteAsync(vehicle);
            }
            catch (Exception)
            {
                _flashMessage.Danger("It is not possible to delete this vehicle because it has services and invoices already registered.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Vehicles/VehicleDetails")]
        public async Task<JsonResult> VehicleDetails(int Id)
        {
            if (Id == 0)
            {
                return null;
            }
            var vehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(Id);
            var vehicleDetails = new VehicleDetailsViewModel
            {
                BrandName = vehicle.Brand.Name,
                ModelName = vehicle.Model.Name,
                DateOfConstruction = vehicle.DateOfConstruction.ToString("dd/MM/yyyy"),
                HorsePower = vehicle.Horsepower.ToString(),
                VIN = vehicle.VehicleIdentificationNumber,
                PlateNumber = vehicle.PlateNumber,
            };
            var json = Json(vehicleDetails);
            return json;
        }

        [HttpPost]
        [Route("Vehicles/GetModelsAsync")]
        public async Task<JsonResult> GetModelsAsync(int brandId)
        {
            if (brandId == 0)
            {
                return Json(new List<object>());
            }

            var brand = await _brandRepository.GetBrandWithModelsAsync(brandId);

            if (brand == null || brand.Models == null || !brand.Models.Any())
            {
                return Json(new List<object>());
            }

            var models = brand.Models
                              .OrderBy(m => m.Name)
                              .Select(m => new { Id = m.Id, Name = m.Name })
                              .ToList();

            return Json(models);
        }

        private bool CheckIfVinIsNull(string vinNumber)
        {
            bool vinIsNull = false;

            if (string.IsNullOrEmpty(vinNumber))
            {
                vinIsNull = true;
            }

            return vinIsNull;
        }
    }
}

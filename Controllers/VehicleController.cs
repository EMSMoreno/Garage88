using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IClientRepository _clientRepository;

        public VehicleController(IVehicleRepository vehicleRepository, IClientRepository clientRepository)
        {
            _vehicleRepository = vehicleRepository;
            _clientRepository = clientRepository;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = new SelectList(await _clientRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _vehicleRepository.AddAsync(vehicle);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = new SelectList(await _clientRepository.GetAllAsync(), "Id", "Name");
            return View(vehicle);
        }
    }

}

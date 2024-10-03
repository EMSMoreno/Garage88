using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Controllers
{
    public class MechanicController : Controller
    {
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public MechanicController(IMechanicRepository mechanicRepository, IClientRepository clientRepository, IVehicleRepository vehicleRepository)
        {
            _mechanicRepository = mechanicRepository;
            _clientRepository = clientRepository;
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Clients = new SelectList(await _clientRepository.GetAllAsync(), "Id", "Name");
            ViewBag.Vehicles = new SelectList(await _vehicleRepository.GetAllAsync(), "Id", "Model");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Mechanic mechanic)
        {
            if (ModelState.IsValid)
            {
                await _mechanicRepository.AddAsync(mechanic);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Clients = new SelectList(await _clientRepository.GetAllAsync(), "Id", "Name");
            ViewBag.Vehicles = new SelectList(await _vehicleRepository.GetAllAsync(), "Id", "Model");
            return View(mechanic);
        }
    }

}

using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Controllers
{
    public class RepairController : Controller
    {
        private readonly IRepairRepository _repairRepository;
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IClientRepository _clientRepository;

        public RepairController(IRepairRepository repairRepository, IMechanicRepository mechanicRepository, IClientRepository clientRepository)
        {
            _repairRepository = repairRepository;
            _mechanicRepository = mechanicRepository;
            _clientRepository = clientRepository;
        }

        public async Task<IActionResult> Index()
        {
            var repairs = await _repairRepository.GetAllRepairsAsync();
            return View(repairs);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Mechanics = new SelectList(await _mechanicRepository.GetAllAsync(), "Id", "Name");
            ViewBag.Clients = new SelectList(await _clientRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Repair repair)
        {
            if (ModelState.IsValid)
            {
                await _repairRepository.AddAsync(repair);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Mechanics = new SelectList(await _mechanicRepository.GetAllAsync(), "Id", "Name");
            ViewBag.Clients = new SelectList(await _clientRepository.GetAllAsync(), "Id", "Name");
            return View(repair);
        }
    }

}

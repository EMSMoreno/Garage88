using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Controllers
{
    public class BillingController : Controller
    {
        private readonly IBillingRepository _billingRepository;
        private readonly IRepairRepository _repairRepository;

        public BillingController(IBillingRepository billingRepository, IRepairRepository repairRepository)
        {
            _billingRepository = billingRepository;
            _repairRepository = repairRepository;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Repairs = new SelectList(await _repairRepository.GetAllAsync(), "Id", "Description");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Billing billing)
        {
            if (ModelState.IsValid)
            {
                await _billingRepository.AddAsync(billing);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Repairs = new SelectList(await _repairRepository.GetAllAsync(), "Id", "Description");
            return View(billing);
        }
    }

}

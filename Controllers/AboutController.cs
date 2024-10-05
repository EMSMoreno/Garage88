using Garage88.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Garage88.Controllers
{
    public class AboutController : Controller
    {
        private readonly IMechanicRepository _mechanicRepository;

        public AboutController(IMechanicRepository mechanicRepository)
        {
            _mechanicRepository = mechanicRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Technicians()
        {
            var mechanics = await _mechanicRepository.GetTechniciansMechanicsAsync();

            return View(mechanics);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Repositories;
using System.Threading.Tasks;

namespace Garage88.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrdersController : ControllerBase
    {
        private readonly IWorkOrderRepository _workOrderRepository;

        public WorkOrdersController(IWorkOrderRepository workOrderRepository)
        {
            _workOrderRepository = workOrderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkOrders(string plateNumber)
        {
            // Verifies if plate number is null or empty
            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                return BadRequest("Plate number is required.");
            }

            var results = await _workOrderRepository.GetAllWorkOrdersByPlateNumberAsync(plateNumber);

            // Verify if data was found
            if (results == null || !results.Any())
            {
                return NotFound($"No work orders found for plate number {plateNumber}.");
            }

            return Ok(results); // Return code: 200 OK w/results
        }
    }
}

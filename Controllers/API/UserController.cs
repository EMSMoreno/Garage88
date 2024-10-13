using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Repositories;
using System.Threading.Tasks;

namespace Garage88.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public UserController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string email)
        {
            var client = await _clientRepository.GetClientByEmailAsync(email);

            if (client == null)
            {
                return NotFound($"User with email {email} was not found.");
            }

            return Ok(client);
        }
    }
}

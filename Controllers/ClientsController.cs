using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Vereyon.Web;
using Garage88.Data.Entities;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin, Client, Receptionist")]
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IFlashMessage _flashMessage;

        public ClientsController(IClientRepository clientRepository,
                                 IUserHelper userHelper,
                                 IMailHelper mailHelper,
                                 IVehicleRepository vehicleRepository,
                                 IBrandRepository brandRepository,
                                 IFlashMessage flashMessage)
        {
            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _flashMessage = flashMessage;
        }

        [Authorize(Roles = "Admin, Client")]
        public IActionResult Index()
        {
            var clients = _clientRepository.GetAll().OrderBy(c => c.FirstName);
            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                bool userExists = false;
                Response isSent = new Response();

                if (user != null)
                {
                    userExists = true;
                }

                var client = await _clientRepository.GetClientByEmailAsync(model.UserName);

                if (client != null)
                {
                    ModelState.AddModelError(string.Empty, "Client email already in use.");
                    return View(model);
                }

                if (!userExists)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Email = model.UserName,
                        UserName = model.UserName,
                        Address = model.Address
                    };

                    var response = await _userHelper.AddUserAsync(user, "DefaultPassword123");

                    if (!response.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to create User");
                        return View(model);
                    }

                    var result = await _userHelper.AddUserToRoleAsync(user, "Client");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created, failed to assign as client");
                        return View(model);
                    }

                    string userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = userToken
                    }, protocol: HttpContext.Request.Scheme);

                    isSent = await _mailHelper.SendEmail(model.UserName, "Welcome to Garage88", $"<h1>Email Confirmation</h1>" +
                    $"Welcome to Garage88!</br></br>Since you have ordered a service with the best auto shop in Lisbon we created you an account!</br>" +
                    $"To allow you to access the website, " +
                    $"please click in the following link to finish the process:<a href= \"{tokenLink}\"> Confirm Email </a>", null);
                }

                client = new Client
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Email = model.UserName,
                    User = user,
                    Nif = model.Nif,
                    PhoneNumber = model.PhoneNumber,
                };

                try
                {
                    await _clientRepository.CreateAsync(client);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }

                var confirmNewClientExists = await _clientRepository.CheckIfClientInBdByEmailAsync(model.UserName);

                if (!confirmNewClientExists)
                {
                    ModelState.AddModelError(string.Empty, "The client couldn't be created, failed to add to Database");
                    return View(model);
                }

                if (isSent.IsSuccess)
                {
                    _flashMessage.Confirmation($"Client account has been created and an email has been sent to {user.Email}, please inform the client about account creation!");
                    return RedirectToAction("Create", "Vehicles", new { email = user.Email, isEstimate = true });

                }
                else
                {
                    _flashMessage.Warning($"Client has been created! But failed to send the confirmation email to the client!");
                    return RedirectToAction("Create", "Vehicles", new { email = user.Email, isEstimate = true });
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // posso alterar para mostrar algum erro personalizado
            }

            var client = await _clientRepository.GetClientWithUserByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound(); // posso alterar para mostrar algum erro personalizado
            }

            var model = new EditClientViewModel
            {
                Address = client.Address,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Nif = client.Nif,
                PhoneNumber = client.PhoneNumber,
                Email = client.Email,
                UserId = client.User.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _clientRepository.GetClientByUserIdAsync(model.UserId);

                if (client == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error updating client info. Client not found.");
                    return View(model);
                }

                client.Address = model.Address;
                client.Email = model.Email;
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.PhoneNumber = model.PhoneNumber;
                client.Nif = model.Nif;

                try
                {
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientWithUserByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            if (client.Vehicles.Count > 0)
            {
                foreach (var vehicle in client.Vehicles)
                {
                    var clientVehicle = await _vehicleRepository.GetVehicleDetailsByIdAsync(vehicle.Id);

                    vehicle.Brand = clientVehicle.Brand;
                    vehicle.Model = clientVehicle.Model;
                }
            }

            string profilePictureUrl = "/images/blankprofilepicture.jpg";

            if (client.ProfilePictureId != null)
            {
                profilePictureUrl = Url.Content($"~/images/{client.ProfilePictureId}.jpg");
            }

            ViewBag.ProfilePictureUrl = profilePictureUrl;

            return View(client);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientWithUserByIdAsync(id.Value);


            if (client == null)
            {
                return NotFound();
            }

            try
            {
                await _clientRepository.DeleteAsync(client);
            }
            catch (Exception)
            {
                _flashMessage.Danger("It is not possible to delete this client. There is an estimate/work Order already going on. If you want to remove the client, please see client history and remove any related files.");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

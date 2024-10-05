using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vereyon.Web;

namespace Garage88.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IBlobHelper _blobHelper;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IConfiguration _configuration;

        public AccountController(IUserHelper userHelper, IMailHelper mailHelper, IClientRepository clientRepository,
            IFlashMessage flashMessage, IBlobHelper blobHelper, IInvoiceRepository invoiceRepository, IVehicleRepository vehicleRepository,
            IMechanicRepository mechanicRepository, IConfiguration configuration)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _clientRepository = clientRepository;
            _flashMessage = flashMessage;
            _blobHelper = blobHelper;
            _invoiceRepository = invoiceRepository;
            _vehicleRepository = vehicleRepository;
            _mechanicRepository = mechanicRepository;
            _configuration = configuration;
        }

        public IActionResult Login()
        {

            if (User.Identity.IsAuthenticated)
            {

                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {

                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                    if (user != null)
                    {
                        var isClientRole = await _userHelper.CheckUserInRoleAsync(user, "Client");

                        if (isClientRole)
                        {
                            return this.RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "DashboardPanel");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Failed to Login");
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnurl = null)
        {
            var redirect = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnurl });
            var properties = _userHelper.ConfigureExternalAuthenticationProperties(provider, redirect);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnurl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, "Error from external provider");
                return View("Login");
            }

            var info = await _userHelper.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _userHelper.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                await _userHelper.UpdateExternalAuthenticationTokensAsync(info);
                if (returnurl != null)
                {
                    return LocalRedirect(returnurl);
                }
                else return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["ReturnUrl"] = returnurl;
                ViewData["ProvierDisplayName"] = info.ProviderDisplayName;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string? returnurl = null)
        {
            returnurl = returnurl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var info = await _userHelper.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("Error");
                }
                var user = new User
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                };

                var result = await _userHelper.CreateAsync(user);

                if (result.Succeeded)
                {
                    var client = new Client
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        User = user
                    };

                    await _clientRepository.CreateAsync(client);

                    result = await _userHelper.AddUserToRoleAsync(user, "Client");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created, failed to assign as client");
                        return View(model);
                    }

                    var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    await _userHelper.ConfirmEmailAsync(user, token);
                    await _userHelper.UpdateUserAsync(user);

                    var loginResult = await _userHelper.AddLoginAsync(user, info);

                    if (loginResult.Succeeded)
                    {
                        await _userHelper.SignInAsync(user, isPersistent: false);
                        await _userHelper.UpdateExternalAuthenticationTokensAsync(info);
                        return LocalRedirect(returnurl);
                    }
                }
                ModelState.AddModelError("Email", "User already exists");
            }

            ViewData["ReturnUrl"] = returnurl;
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        UserName = model.UserName,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                    };


                    var result = await _userHelper.AddUserAsync(user, "DefaultPassword123");

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    var client = new Client
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        Address = model.Address,
                        User = user
                    };

                    await _clientRepository.CreateAsync(client);

                    result = await _userHelper.AddUserToRoleAsync(user, "Client");

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

                    Response response = await _mailHelper.SendEmail(model.UserName, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $" To allow you to access the website, " +
                        $"please click in the following link:</br></br><a href= \"{tokenLink}\">Confirm Email </a>", null);

                    if (response.IsSuccess)
                    {
                        ViewBag.Message = $"An email has been sent to {user.Email}, please check your email and follow the instructions.";
                        return View(model);
                    }

                }

            }

            ModelState.AddModelError(string.Empty, "The user couldn't be created, probably Email is alredy registered.");

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return NotFound();
            }


            var model = new AddUserPasswordViewModel
            {
                UserId = userId,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(AddUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, "DefaultPassword123", model.Password);
                    if (result.Succeeded)
                    {

                        ViewBag.Success = "You can now log in into the system.";
                        return View(model);

                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "That User was not found!");
                }
            }
            return View(model);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The Email does not correspond to a registered email.");
                    return View(model);
                }

                var userToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account", new { token = userToken, userId = user.Id }, protocol: HttpContext.Request.Scheme);

                Response response = await _mailHelper.SendEmail(model.Email, "PitStop Lisbon Recover Password ", $"<h1>PitStop Lisbon password reset</h1>" +
                    $"To Reset the password click in the link bellow: </br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>", null);

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password have been sent to the email address.";
                }

                return View();
            }

            return View(model);
        }

        public async Task<IActionResult> ResetPassword(string token, string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ResetPasswordViewModel
            {
                UserName = user.UserName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                if (user != null)
                {
                    var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if (user.EmailConfirmed == false)
                        {
                            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                            await _userHelper.ConfirmEmailAsync(user, token);
                        }

                        ViewBag.Message = "Password Reset Successful, you can now Log in with your new credentials.";
                        return View();
                    }


                    ViewBag.Message = "There was an error while doing reset to your password.";
                    return View(model);
                }

                ViewBag.Message = "User was not found!";
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [Route("Account/ChangePassword")]
        public async Task<JsonResult> ChangePassword(string oldPassword, string newPassword, string repeatedPassword)
        {
            Response response;

            if (newPassword != repeatedPassword)
            {
                response = new Response
                {
                    IsSuccess = false,
                    Message = "New password is not equivalent to the repeated password."
                };

                return Json(response);
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return Json(new Response
                {
                    IsSuccess = false,
                    Message = "User is not valid",
                });
            }

            var isActualUser = await _userHelper.CheckPasswordAsync(user, oldPassword);

            if (isActualUser.Succeeded)
            {

                var result = await _userHelper.ChangePasswordAsync(user, oldPassword, newPassword);
                if (result.Succeeded)
                {
                    return Json(new Response { IsSuccess = true, Message = "Password was succefully changed!" });
                }
                else
                {
                    return Json(new Response
                    {
                        IsSuccess = false,
                        Message = "There was a problem changing the password.",
                    });
                }
            }

            return Json(new Response
            {
                IsSuccess = false,
                Message = "The old password is not correct.",
            });

        }

        [Authorize]
        public async Task<IActionResult> ViewUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var hasPassword = await _userHelper.HasPasswordAsync(user);


            ChangeUserViewModel model = null;
            bool isClient = false;

            if (this.User.IsInRole("Client"))
            {
                var client = await _clientRepository.GetClientByUserIdAsync(user.Id);
                isClient = true;
                if (client == null)
                {
                    return NotFound();
                }

                model = new ChangeUserViewModel
                {
                    PhoneNumber = client.PhoneNumber,
                    FirstName = client.FirstName,
                    Address = client.Address,
                    Email = client.Email,
                    LastName = client.LastName,
                    Nif = client.Nif,
                    ProfilePicture = user.ProfilePicture,
                    HasPassword = hasPassword,
                };


            }
            else
            {

                model = new ChangeUserViewModel
                {
                    PhoneNumber = user.PhoneNumber,
                    LastName = user.LastName,
                    FirstName = user.FirstName,
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfilePicture = user.ProfilePicture,
                    Address = user.Address,
                    HasPassword = hasPassword,
                };

            }

            ViewBag.JsonModel = JsonConvert.SerializeObject(model);
            ViewBag.IsCustomer = JsonConvert.SerializeObject(isClient);

            Random r = new Random();
            string[] images = new string[5] { "/images/siteContent/ponte.jpg", "/images/siteContent/recoverPassword.jpg", "/images/siteContent/teste.jpg",
                 "/images/siteContent/karts.jpg", "/images/siteContent/city.jpg" };
            string selectedImage = images[r.Next(5)];
            ViewBag.Image = selectedImage;

            return View(model);
        }

        public async Task<IActionResult> ServiceHistory()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientByUserIdAsync(user.Id);

            if (client == null)
            {
                return NotFound();
            }

            var invoiceHistory = await _invoiceRepository.GetUserInvoicesAsync(client.Id);

            return View(invoiceHistory);

        }

        public async Task<IActionResult> InvoiceDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetInvoiceDetailsByIdAsync(id.Value);

            if (invoice == null)
            {
                return NotFound();
            }


            return View(invoice);
        }

        public async Task<IActionResult> Vehicles()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientByUserIdAsync(user.Id);

            if (client == null)
            {
                return NotFound();
            }

            var vehicles = _vehicleRepository.GetClientVehiclesAsync(client.Id);

            return View(vehicles);

        }

        [HttpPost]
        [Route("Account/GetProfilePicturePath")]
        public async Task<JsonResult> GetProfilePicturePath()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            var json = Json(user);
            return json;
        }

        [HttpPost]
        [Route("Account/ChangeProfilePic")]
        public async Task<IActionResult> ChangeProfilePic(IFormFile file)
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user != null && file != null)
            {

                Guid imageId = user.ProfilePicture;

                if (file != null && file.Length > 0)
                {


                    using var image = Image.Load(file.OpenReadStream());
                    image.Mutate(img => img.Resize(256, 0));

                    using (MemoryStream m = new MemoryStream())
                    {
                        image.SaveAsJpeg(m);
                        byte[] imageBytes = m.ToArray();
                        imageId = await _blobHelper.UploadBlobAsync(imageBytes, "profilepictures");
                    }
                }

                user.ProfilePicture = imageId;

                if (!this.User.IsInRole("Client") && !this.User.IsInRole("Admin"))
                {
                    var employee = await _mechanicRepository.GetByEmailAsync(User.Identity.Name);
                    if (employee == null)
                    {
                        return new ObjectResult(new { Status = "fail" });
                    }

                    employee.PhotoId = imageId;
                    try
                    {
                        await _mechanicRepository.UpdateAsync(employee);
                    }
                    catch (Exception)
                    {
                        return new ObjectResult(new { Status = "fail" });
                    }
                }

                var response = await _userHelper.UpdateUserAsync(user);

                if (!response.Succeeded)
                {
                    _flashMessage.Danger("There was an error updating the profile picture.");

                    return new ObjectResult(new { Status = "fail" });
                }

                return new ObjectResult(new { Status = "success" });
            }


            return new ObjectResult(new { Status = "fail" });
        }

        [HttpPost]
        [Route("Account/UpdateUser")]
        public async Task<JsonResult> UpdateUser(string email, long phoneNumber, long nif, string address, bool isClient)
        {
            bool isValid = false;
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return Json(isValid);
            }

            if (isClient)
            {
                var client = await _clientRepository.GetClientByUserIdAsync(user.Id);
                if (client == null)
                {
                    return Json(isValid);
                }
                user.Email = email;
                user.UserName = email;
                client.Email = email;
                client.PhoneNumber = phoneNumber.ToString();
                client.Address = address;
                client.Nif = nif.ToString();

                try
                {
                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception)
                {
                    return Json(isValid);
                }
            }

            user.Address = address;
            user.PhoneNumber = phoneNumber.ToString();

            try
            {
                await _userHelper.UpdateUserAsync(user);

                isValid = true;
            }
            catch (Exception)
            {
                return Json(isValid);
            }

            return Json(isValid);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (user != null)
                {
                    var result = await _userHelper.CheckPasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(_configuration["Tokens:Issuer"], _configuration["Tokens:Audience"], claims, expires: DateTime.UtcNow.AddDays(15), signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }

                }

            }

            return this.BadRequest();
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}

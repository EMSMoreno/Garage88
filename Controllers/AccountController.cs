﻿using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                    var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                    Console.WriteLine($"User {model.UserName} has roles: {string.Join(", ", user)}");
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
                    ModelState.AddModelError(string.Empty, "Failed to Login, try again later!");
                }
            }

            ModelState.AddModelError(string.Empty, "Failed to login");     
            return View(model);
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnurl = null)
        //{
        //    var redirect = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnurl });
        //    var properties = _userHelper.ConfigureExternalAuthenticationProperties(provider, redirect);
        //    return Challenge(properties, provider);
        //}

        //[HttpGet]
        //public async Task<IActionResult> ExternalLoginCallback(string returnurl = null, string remoteError = null)
        //{
        //    if (remoteError != null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Error from external provider");
        //        return View("Login");
        //    }

        //    var info = await _userHelper.GetExternalLoginInfoAsync();

        //    if (info == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    var result = await _userHelper.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

        //    if (result.Succeeded)
        //    {
        //        await _userHelper.UpdateExternalAuthenticationTokensAsync(info);
        //        if (returnurl != null)
        //        {
        //            return LocalRedirect(returnurl);
        //        }
        //        else return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        ViewData["ReturnUrl"] = returnurl;
        //        ViewData["ProvierDisplayName"] = info.ProviderDisplayName;
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //        return View("ExternalLoginConfirmation", new ExternalLoginViewModel { Email = email });
        //    }
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string? returnurl = null)
        //{
        //    returnurl = returnurl ?? Url.Content("~/");

        //    if (ModelState.IsValid)
        //    {
        //        var info = await _userHelper.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("Error");
        //        }
        //        var user = new User
        //        {
        //            Email = model.Email,
        //            FirstName = model.FirstName,
        //            LastName = model.LastName,
        //            UserName = model.Email,
        //        };

        //        var result = await _userHelper.CreateAsync(user);

        //        if (result.Succeeded)
        //        {
        //            var client = new Client
        //            {
        //                FirstName = model.FirstName,
        //                LastName = model.LastName,
        //                Email = model.Email,
        //                User = user
        //            };

        //            await _clientRepository.CreateAsync(client);

        //            result = await _userHelper.AddUserToRoleAsync(user, "Client");

        //            if (!result.Succeeded)
        //            {
        //                ModelState.AddModelError(string.Empty, "The user couldn't be created, failed to assign as client");
        //                return View(model);
        //            }

        //            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
        //            await _userHelper.ConfirmEmailAsync(user, token);
        //            await _userHelper.UpdateUserAsync(user);

        //            var loginResult = await _userHelper.AddLoginAsync(user, info);

        //            if (loginResult.Succeeded)
        //            {
        //                await _userHelper.SignInAsync(user, isPersistent: false);
        //                await _userHelper.UpdateExternalAuthenticationTokensAsync(info);
        //                return LocalRedirect(returnurl);
        //            }
        //        }
        //        ModelState.AddModelError("Email", "User already exists");
        //    }

        //    ViewData["ReturnUrl"] = returnurl;
        //    return View(model);
        //}

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

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tenta buscar o usuário
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (user == null)
                {
                    // Cria o objeto do usuário
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        UserName = model.UserName,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                    };

                    // Tenta adicionar o usuário
                    var result = await _userHelper.AddUserAsync(user, GenerateRandomPassword());

                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }

                    // Cria e associa o cliente ao usuário
                    var client = new Client
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.UserName,
                        Address = model.Address,
                        User = user
                    };

                    await _clientRepository.CreateAsync(client);

                    // Adiciona o papel ao usuário
                    result = await _userHelper.AddUserToRoleAsync(user, "Client");
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to assign the user as a client.");
                        return View(model);
                    }

                    // Gera o token de confirmação
                    string userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = userToken
                    }, protocol: HttpContext.Request.Scheme);

                    // Envia o e-mail de confirmação
                    Response response = await _mailHelper.SendEmail(
                        model.UserName,                     // Para
                        "Email confirmation",               // Assunto
                        $"<h1>Email Confirmation</h1>" +    // Corpo
                        $"Please confirm your email by clicking <a href=\"{tokenLink}\">here</a>.",
                        null                                // Sem anexos
                    );

                    if (!response.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to send confirmation email.");
                        return View(model);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered.");
                }
            }

            return View(model);
        }

        private string GenerateRandomPassword()
        {
            return "GeneratedPassword123";
        }

        public IActionResult GenerateHash()
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var newPassword = "GeneratedPassword123";
            var hash = passwordHasher.HashPassword(null, newPassword);

            return Content(hash);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            //if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            //{
            //    return NotFound();
            //}

            //var user = await _userHelper.GetUserByIdAsync(userId);

            //if (user == null)
            //{
            //    return NotFound();
            //}

            //var result = await _userHelper.ConfirmEmailAsync(user, token);

            //if (!result.Succeeded)
            //{
            //    return NotFound();
            //}


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
                    var result = await _userHelper.ChangePasswordAsync(user, "GeneratedPassword123", model.Password);
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

        // GET: Account/RecoverPassword
        public IActionResult RecoverPassword()
        {
            return View();
        }

        // POST: Account/RecoverPassword
        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email does not correspond to a registered email.");
                    return View(model);
                }

                var userToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account", new { token = userToken, userId = user.Id }, protocol: HttpContext.Request.Scheme);

                Response response = await _mailHelper.SendEmail(model.Email, "Garage88 Recover Password",
                    $"<h1>Garage88 password reset</h1>" +
                    $"To reset the password, click the link below: </br></br>" +
                    $"<a href=\"{link}\">Reset Password</a>", null);

                if (response.IsSuccess)
                {
                    ViewBag.Message = "The instructions to recover your password have been sent to the email address.";
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There was a problem sending the email. Please try again later.");
                    return View(model);
                }
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
                        if (!user.EmailConfirmed)
                        {
                            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                            await _userHelper.ConfirmEmailAsync(user, token);
                        }

                        return RedirectToAction("Login", "Account");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found!");
                }
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

            bool isClient = User.IsInRole("Client");
            var model = new ChangeUserViewModel
            {
                //FullName = $"{user.FirstName} {user.LastName}",
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                //Nif = user.Nif,
                NewPassword = string.Empty,
                HasPassword = user.PasswordHash != null
            };

            ViewBag.Image = user.ProfilePicture != Guid.Empty ? user.ProfilePicture.ToString() : "/images/blankProfilePicture.jpg";

            ViewBag.JsonModel = JsonConvert.SerializeObject(model);
            ViewBag.IsClient = JsonConvert.SerializeObject(isClient);

            Random r = new Random();
            string[] images = new string[4] { "/images/siteContent/ponte.jpg", "/images/siteContent/recoverPassword.jpg", "/images/siteContent/teste.jpg",
             "/images/siteContent/city.jpg" };
            string selectedImage = images[r.Next(images.Length)];
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
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return Json(new { Status = "error", Message = "User not authenticated." });
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return Json(new { Status = "error", Message = "User not found." });
            }

            // Return image path or ID
            return Json(new { Status = "success", ProfilePictureId = user.ProfilePicture });
        }

        [HttpPost]
        [Route("Account/ChangeProfilePic")]
        public async Task<ObjectResult> ChangeProfilePic(IFormFile file)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return BadRequest(new { Status = "error", Message = "User not authenticated." });
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user != null && file != null && file.Length > 0)
            {
                try
                {
                    // Type of file
                    if (!file.ContentType.StartsWith("image/"))
                    {
                        return BadRequest(new { Status = "fail", Message = "Uploaded file is not an image." });
                    }

                    const long maxFileSize = 5 * 1024 * 1024; // 5 MB
                    if (file.Length > maxFileSize)
                    {
                        return BadRequest(new { Status = "fail", Message = "Image size exceeds the limit." });
                    }

                    Guid imageId = user.ProfilePicture;

                    using var image = Image.Load(file.OpenReadStream());

                    int maxWidth = 256;
                    int maxHeight = 256;
                    image.Mutate(img => img.Resize(new ResizeOptions
                    {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.Max // Redimensiona mantendo a proporção, sem esticar
                    }));

                    using (MemoryStream m = new MemoryStream())
                    {
                        image.SaveAsJpeg(m);
                        byte[] imageBytes = m.ToArray();
                        imageId = await _blobHelper.UploadBlobAsync(imageBytes, "profilepictures");
                    }

                    user.ProfilePicture = imageId;
                    var response = await _userHelper.UpdateUserAsync(user);

                    if (response.Succeeded)
                    {
                        return Ok(new { Status = "success", ImageId = imageId });
                    }
                }
                catch (Exception)
                {
                    return BadRequest(new { Status = "fail", Message = "Error processing the image." });
                }
            }

            return BadRequest(new { Status = "fail", Message = "Invalid input." });
        }

        [HttpGet]
        [Route("Account/UpdateUser")]
        public async Task<IActionResult> UpdateUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var model = new ChangeUserViewModel();

            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.PhoneNumber = user.PhoneNumber;
                model.ProfilePicture = user.ProfilePicture;
                model.Email = user.Email;
                model.Address = user.Address;
                model.HasPassword = await _userHelper.HasPasswordAsync(user);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Account/UpdateUser")]
        public async Task<IActionResult> UpdateUser(ChangeUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.ProfilePicture = model.ProfilePicture;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var token = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var result = await _userHelper.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to change password.");
                    return View(model);
                }
            }

            if (model.IsClient)
            {
                var client = await _clientRepository.GetClientByUserIdAsync(user.Id);
                if (client != null)
                {
                    client.PhoneNumber = model.PhoneNumber;
                    client.Address = model.Address;

                    await _clientRepository.UpdateAsync(client);
                }
            }

            try
            {
                await _userHelper.UpdateUserAsync(user);
                TempData["Message"] = "User updated successfully!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to update user.");
                return View(model);
            }
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

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View("Error404");
        }

        [Route("error/401")]
        public IActionResult Error401()
        {
            return View("Error401");
        }
    }
}

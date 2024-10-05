using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Vereyon.Web;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MechanicsController : Controller
    {
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMechanicsRolesRepository _mechanicsRolesRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public MechanicsController(IMechanicRepository mechanicRepository
                                  , IUserHelper userHelper
                                  , IConverterHelper converterHelper
                                  , IMechanicsRolesRepository mechanicsRolesRepository
                                  , IMailHelper mailHelper
                                  , IFlashMessage flashMessage)
        {
            _mechanicRepository = mechanicRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _mechanicsRolesRepository = mechanicsRolesRepository;
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            var mechanics = _mechanicRepository.GetAllWithUsers();

            return View(mechanics);
        }

        public IActionResult ManageRoles()
        {
            var roles = _mechanicsRolesRepository.GetRolesWithSpecialities();

            return View(roles);
        }

        public IActionResult CreateRole()
        {
            var model = new RoleViewModel
            {
                Permissions = _userHelper.GetComboExistingRoles()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            model.Permissions = _userHelper.GetComboExistingRoles();

            if (ModelState.IsValid)
            {

                if (model.SelectedPermission == "0")
                {
                    ModelState.AddModelError(string.Empty, "You must select a permission level.");
                    return View(model);
                }

                var roleName = await _userHelper.GetRoleNameByRoleIdAsync(model.SelectedPermission);

                if (string.IsNullOrEmpty(roleName))
                {
                    _flashMessage.Danger("There was an error adding the permission level");
                    return View(model);
                }

                model.SelectedPermission = roleName;

                var role = _converterHelper.toRole(model, true);
                //var role = new Role
                //{
                //    Name = model.Name,
                //    PermissionsName = roleName
                //};

                try
                {
                    await _mechanicsRolesRepository.CreateAsync(role);
                    return RedirectToAction("ManageRoles", "Mechanics");
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error creating the role. " + ex.InnerException.Message);
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EditRole(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _mechanicsRolesRepository.GetRoleWithSpecialitiesAsync(id.Value);

            if (role == null)
            {
                return NotFound();
            }

            var model = _converterHelper.toRoleViewModel(role);

            //convert from roleName to roleId so the dropdown menu in the ViewModel is automatically filled with the current roleName/PermissionLevel
            model.SelectedPermission = await _userHelper.GetRoleIdWithRoleNameAsync(model.SelectedPermission);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel model)
        {
            model.Permissions = _userHelper.GetComboExistingRoles();

            if (ModelState.IsValid)
            {
                if (model.SelectedPermission == "0")
                {
                    ModelState.AddModelError(string.Empty, "You must select a permission level.");
                    return View(model);
                }

                var roleToEdit = await _mechanicsRolesRepository.GetByIdAsync(model.RoleId);

                if (roleToEdit == null)
                {
                    return NotFound();
                }

                var roleName = await _userHelper.GetRoleNameByRoleIdAsync(model.SelectedPermission);

                if (string.IsNullOrEmpty(roleName))
                {
                    _flashMessage.Danger("There was an error adding the permission level");
                    return View(model);
                }

                roleToEdit.Name = model.Name;
                roleToEdit.PermissionsName = roleName;

                try
                {
                    await _mechanicsRolesRepository.UpdateAsync(roleToEdit);
                    return RedirectToAction(nameof(ManageRoles));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating the role. " + ex.InnerException.Message);
                    return View(model);
                }

            }

            return View(model);
        }

        public async Task<IActionResult> DeleteRole(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _mechanicsRolesRepository.GetRoleWithSpecialitiesAsync(id.Value);

            if (role == null)
            {
                return NotFound();
            }

            try
            {
                await _mechanicsRolesRepository.DeleteAsync(role);
                _flashMessage.Confirmation("Role was deleted with success.");
            }
            catch (Exception ex)
            {
                _flashMessage.Warning("There was an error deleting the role. " + ex.InnerException.Message);
            }

            return RedirectToAction(nameof(ManageRoles));
        }

        public async Task<IActionResult> RoleDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _mechanicsRolesRepository.GetRoleWithSpecialitiesAsync(id.Value);

            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        public async Task<IActionResult> AddSpeciality(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _mechanicsRolesRepository.GetRoleWithSpecialitiesAsync(id.Value);

            if (role == null)
            {
                return NotFound();
            }

            var model = new SpecialityViewModel
            {
                RoleId = role.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddSpeciality(SpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _mechanicsRolesRepository.AddSpecialityAsync(model);
                return RedirectToAction("RoleDetails", new { id = model.RoleId });
            }

            return View(model);
        }

        public async Task<IActionResult> EditSpeciality(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speciality = await _mechanicsRolesRepository.GetSpecialityAsync(id.Value);

            if (speciality == null)
            {
                return NotFound();
            }

            var roleId = await _mechanicsRolesRepository.GetRoleIdWithSpecialityAsync(speciality.Id);

            if (roleId == 0)
            {
                return NotFound();
            }

            var specialityToEdit = new SpecialityViewModel
            {
                Name = speciality.Name,
                RoleId = roleId,
                SpecialityId = speciality.Id
            };

            return View(specialityToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> EditSpecialty(SpecialityViewModel model)
        {
            if (ModelState.IsValid)
            {

                var modelToChange = await _mechanicsRolesRepository.GetSpecialityAsync(model.SpecialityId);

                if (modelToChange == null)
                {
                    ModelState.AddModelError(string.Empty, "There was an error updating the speciality.");
                    return View(model);
                }

                modelToChange.Name = model.Name;

                try
                {
                    await _mechanicsRolesRepository.UpdateSpecialityAsync(modelToChange);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.InnerException.Message);
                    return View(model);
                }

                return RedirectToAction($"RoleDetails", new { id = model.RoleId });
            }

            return View(model);
        }

        public async Task<IActionResult> DeleteSpecialty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speciality = await _mechanicsRolesRepository.GetSpecialityAsync(id.Value);

            if (speciality == null)
            {
                return NotFound();
            }

            var roleId = await _mechanicsRolesRepository.GetRoleIdWithSpecialityAsync(speciality.Id);

            if (roleId == 0)
            {
                return NotFound();
            }

            try
            {
                await _mechanicsRolesRepository.DeleteSpecialityAsync(speciality);
                return RedirectToAction($"RoleDetails", new { id = roleId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException.Message);
            }

            return RedirectToAction($"RoleDetails", new { id = roleId });
        }

        public IActionResult Create()
        {

            var model = new MechanicViewModel
            {
                Roles = _mechanicsRolesRepository.GetComboRoles(),
                Specialities = _mechanicsRolesRepository.GetComboSpeciality(0)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MechanicViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    ProfilePicture = new Guid(),
                };

                var result = await _userHelper.AddUserAsync(newUser, "DefaultPassword123");

                if (!result.Succeeded)
                {
                    _flashMessage.Danger("There was an error creating the Mechanic user data.");
                    return View(model);
                }

                var mechanic = await _converterHelper.ToMechanic(model, newUser, true);

                if (mechanic == null)
                {
                    _flashMessage.Danger("There was an error creating the mechanic.");
                    return View(model);
                }

                result = await _userHelper.AddUserToRoleAsync(newUser, mechanic.Role.PermissionsName);

                if (!result.Succeeded)
                {
                    _flashMessage.Danger("There was an error adding the user to the required permission level.");
                    return View(model);
                }

                var userToken = await _userHelper.GenerateEmailConfirmationTokenAsync(newUser);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = newUser.Id,
                    token = userToken
                }, protocol: HttpContext.Request.Scheme);

                Response isSent = await _mailHelper.SendEmail(model.Email, "Welcome to Garage88", $"<h1>Email Confirmation</h1>" +
                   $"Welcome to Garage88!</br></br>First of all congratulations! You are now a new Garage88 mechanic! </br>" +
                   $"To allow you to access the website and the management system, " +
                   $"please click in the following link:<a href= \"{tokenLink}\"> Confirm Email </a>", null);

                if (isSent.IsSuccess)
                {
                    _flashMessage.Confirmation("Mechanic was created with success! Please allow mechanic to know that he needs to confirm the email address!");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _flashMessage.Warning("There was an error sending the confirmation email. But the mechanic has been created. Ask System manager to validate the email address or Delete the mechanic and try adding him later.");
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _mechanicRepository.GetMechanicByIdAsync(id.Value);

            if (mechanic == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToMechanicViewModel(mechanic, false);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MechanicViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);

                if (user == null)
                {
                    _flashMessage.Warning("There was an error updating the mechanic");
                    return View(model);
                }

                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                if (user.Email != model.Email)
                {
                    user.UserName = model.Email;
                    user.Email = model.Email;
                    var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    await _userHelper.ConfirmEmailAsync(user, token);
                }

                var result = await _userHelper.UpdateUserAsync(user);

                if (!result.Succeeded)
                {
                    _flashMessage.Warning("There was an error updating the mechanic user information.");
                    return View(model);
                }

                var mechanic = await _converterHelper.ToMechanic(model, user, false);

                if (mechanic == null)
                {
                    _flashMessage.Warning("There was an error updating the mechanic.");
                    return View(model);
                }

                try
                {
                    await _mechanicRepository.UpdateAsync(mechanic);
                    _flashMessage.Confirmation("Mechanic was sucessfully updated.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating the mechanic. " + ex.InnerException.Message);
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mechanic = await _mechanicRepository.GetMechanicByIdAsync(id.Value);

            if (mechanic == null)
            {
                return NotFound();
            }

            try
            {
                await _mechanicRepository.DeleteAsync(mechanic);
                _flashMessage.Confirmation("Mechanic was sucessufuly deleted.");
            }
            catch (Exception)
            {
                _flashMessage.Danger("There was a problem deleting the mechanic. Probably the mechanic you are trying to delete as already some sort of entry on a client file. You would need to delete all client files with the asked mechanic in them.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Mechanics/GetSpecialitiesAsync")]
        public async Task<JsonResult> GetSpecialitiesAsync(int roleId)
        {
            var role = await _mechanicsRolesRepository.GetRoleWithSpecialitiesAsync(roleId);
            return Json(role.Specialities.OrderBy(s => s.Name));
        }

        [HttpPost]
        [Route("Mechanics/MechanicDetails")]
        public async Task<JsonResult> MechanicDetails(int Id)
        {
            if (Id == 0)
            {
                return null;
            }

            var mechanic = await _mechanicRepository.GetMechanicByIdAsync(Id);

            var json = Json(mechanic);
            return json;
        }
    }
}

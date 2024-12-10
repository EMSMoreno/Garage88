using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Linq;
using System.Data;
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
        public async Task<IActionResult> EditSpeciality(SpecialityViewModel model)
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

        public async Task<IActionResult> DeleteSpeciality(int? id)
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
                User = new User(),
                UserId = string.Empty
            };

            ViewBag.Roles = _mechanicsRolesRepository.GetComboRoles();
            ViewBag.Specialities = _mechanicsRolesRepository.GetComboSpeciality(0);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MechanicViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Step 1: Create the User
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                };

                // Generate a default password
                string defaultPassword = "TemporaryPassword123!";

                // Step 2: Create the user and save to the database
                var result = await _userHelper.AddUserAsync(user, defaultPassword);

                // Check if the user creation failed and log the errors
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    // Repopulate dropdowns if the user creation fails
                    model.Roles = new SelectList(_mechanicsRolesRepository.GetRolesWithSpecialities(), "Id", "Name", model.RoleId);
                    model.Specialities = new SelectList(_mechanicsRolesRepository.GetRolesWithSpecialities(), "Id", "Name", model.SpecialityId);

                    return View(model);
                }

                // Ensure that the user is created and the user.Id is set
                if (user.Id == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create the user.");
                    return View(model);
                }

                // Step 3: Create the Mechanic and associate with the UserId
                var mechanic = new Mechanic
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    About = model.About,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    UserId = user.Id,
                    RoleId = model.RoleId,
                    SpecialityId = model.SpecialityId,
                    Color = string.IsNullOrEmpty(model.Color) ? "#FFFFFF" : model.Color,
                    PhotoId = model.PhotoId
                };

                // Step 4: Save Mechanic in the database
                await _mechanicRepository.AddMechanicAsync(mechanic); // Ensure this is an async method

                // Optionally, assign a role to the User (if not already done in the creation step)
                await _userHelper.AddUserToRoleAsync(user, "Mechanic");

                // Step 5: Redirect to the Index or another view
                return RedirectToAction("Index");
            }

            // Repopulate the roles and specialities dropdowns if validation fails
            model.Roles = new SelectList(_mechanicsRolesRepository.GetRolesWithSpecialities(), "Id", "Name", model.RoleId);
            model.Specialities = new SelectList(_mechanicsRolesRepository.GetRolesWithSpecialities(), "Id", "Name", model.SpecialityId);

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var mechanic = await _mechanicRepository.GetByIdAsync(id);

            if (mechanic == null)
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(mechanic.UserId);
            if (user == null)
            {
                return NotFound("User associated with this mechanic not found.");
            }

            var model = new MechanicViewModel
            {
                MechanicId = mechanic.Id,
                UserId = mechanic.UserId, // Ensure this is set correctly
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                RoleId = mechanic.RoleId,
                SpecialityId = mechanic.SpecialityId,
                Color = mechanic.Color,
                PhotoId = mechanic.PhotoId
            };

            ViewBag.Roles = _mechanicsRolesRepository.GetComboRoles();
            ViewBag.Specialities = _mechanicsRolesRepository.GetComboSpeciality(mechanic.RoleId);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MechanicViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserId))
            {
                ModelState.AddModelError(string.Empty, "UserId is missing or empty.");
                return View(model);
            }

            Console.WriteLine($"UserId: {model.UserId}"); // Log the UserId for debugging

            var user = await _userHelper.GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, $"User not found for UserId: {model.UserId}");
                ViewBag.Roles = _mechanicsRolesRepository.GetComboRoles();
                ViewBag.Specialities = _mechanicsRolesRepository.GetComboSpeciality(model.RoleId);
                return View(model);
            }

            // Atualizar os dados do usuário
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;

            // Verificar se o email foi alterado
            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = model.Email;
                user.UserName = model.Email;

                // Gerar novo token de confirmação, se necessário
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                var confirmationResult = await _userHelper.ConfirmEmailAsync(user, token);

                if (!confirmationResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Error confirming the updated email.");
                    ViewBag.Roles = _mechanicsRolesRepository.GetComboRoles();
                    ViewBag.Specialities = _mechanicsRolesRepository.GetComboSpeciality(model.RoleId);
                    return View(model);
                }
            }

            // Atualizar o usuário no banco
            var result = await _userHelper.UpdateUserAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Recarregar dropdowns em caso de erro
                ViewBag.Roles = _mechanicsRolesRepository.GetComboRoles();
                ViewBag.Specialities = _mechanicsRolesRepository.GetComboSpeciality(model.RoleId);
                return View(model);
            }

            // Atualizar os dados do mecânico
            var mechanic = await _mechanicRepository.GetByIdAsync(model.MechanicId);
            if (mechanic == null)
            {
                ModelState.AddModelError(string.Empty, "Mechanic not found.");
                ViewBag.Roles = _mechanicsRolesRepository.GetComboRoles();
                ViewBag.Specialities = _mechanicsRolesRepository.GetComboSpeciality(model.RoleId);
                return View(model);
            }

            mechanic.FirstName = model.FirstName;
            mechanic.LastName = model.LastName;
            mechanic.Email = model.Email;
            mechanic.Address = model.Address;
            mechanic.PhoneNumber = model.PhoneNumber;
            mechanic.RoleId = model.RoleId;
            mechanic.SpecialityId = model.SpecialityId;
            mechanic.Color = string.IsNullOrEmpty(model.Color) ? "#FFFFFF" : model.Color;
            mechanic.PhotoId = model.PhotoId;

            await _mechanicRepository.UpdateAsync(mechanic);

            // Redirecionar para a lista após atualização
            TempData["SuccessMessage"] = "Mechanic updated successfully!";
            return RedirectToAction("Index");
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

        [HttpGet]
        public IActionResult GetSpecialities(int roleId)
        {
            var specialities = _mechanicsRolesRepository.GetComboSpeciality(roleId)
                               .Where(s => s.Value != "0")
                               .ToList();

            return Json(specialities);
        }

        [HttpPost]
        [Route("Mechanics/MechanicDetails")]
        public async Task<JsonResult> MechanicDetails(int Id)
        {
            if (Id == 0)
            {
                return Json(new { error = "Invalid mechanic ID." });
            }

            var mechanic = await _mechanicRepository.GetMechanicByIdAsync(Id);

            if (mechanic == null)
            {
                return Json(new { error = "Mechanic not found." });
            }

            var mechanicDetails = new
            {
                fullName = mechanic.FullName,
                email = mechanic.Email,
                user = new
                {
                    address = mechanic.User?.Address,
                    phoneNumber = mechanic.User?.PhoneNumber
                },
                role = new { name = mechanic.Role?.Name },
                speciality = new { name = mechanic.Speciality?.Name },
                about = mechanic.About
            };

            return Json(mechanicDetails);
        }
    }
}

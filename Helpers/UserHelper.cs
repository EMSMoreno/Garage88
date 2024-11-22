using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage88.Data.Entities;
using Garage88.Models;
using Garage88.Helpers;

namespace Garage88.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public UserHelper(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info)
        {
            return await _userManager.AddLoginAsync(user, info);
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            var createUserResult = await _userManager.CreateAsync(user, "GeneratedPassword123");

            if (!createUserResult.Succeeded)
            {
                throw new Exception("Error creating user: " + string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
            }

            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<SignInResult> CheckPasswordAsync(User user, string oldPassword)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, oldPassword, false);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName,
                });
            }
        }

        public async Task<bool> CheckUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirect)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirect);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            return await _userManager.CreateAsync(user);
        }

        public async Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            return await _signInManager.ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public IEnumerable<SelectListItem> GetComboExistingRoles()
        {
            var list = _roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id
            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Insert Permission Level]",
                Value = "0"
            });

            return list;
        }

        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            return info;
        }

        public async Task<string> GetRoleIdWithRoleNameAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            return role.Id;
        }

        public async Task<string> GetRoleNameByRoleIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            return await _roleManager.GetRoleNameAsync(role);
        }

        public async Task<int> GetTotalUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            return users.Count;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email.ToLower().Trim());
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<APIUserViewModel> GetUserDetailsAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);

            if (user != null)
            {
                return new APIUserViewModel
                {
                    Address = user.Address,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePicture = user.ProfilePicture
                };
            }

            return null;
        }

        public async Task<List<UserDataChartModel>> GetUsersChartDataAsync()
        {
            List<UserDataChartModel> list = new List<UserDataChartModel>();
            string[] roles = new string[4] { "Client", "Technician", "Admin", "Receptionist" };
            string[] colors = new string[4] { "#181818", "#8758FF", "#5CB8E4", "#F2F2F2" };
            int identer = 0;

            foreach (string role in roles)
            {
                var users = await _userManager.GetUsersInRoleAsync(role);
                list.Add(new UserDataChartModel
                {
                    UserRoleName = role,
                    Quantity = users.Count(),
                    Color = colors[identer]
                });
                identer++;
            }

            return list.OrderBy(l => l.UserRoleName).ToList();
        }

        public async Task<bool> HasPasswordAsync(User user)
        {
            return await _userManager.HasPasswordAsync(user);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task SignInAsync(User user, bool isPersistent)
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }

        public async Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info)
        {
            return await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }
    }
}

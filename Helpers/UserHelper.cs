using Garage88.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        public Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddUserAsync(User user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddUserToRoleAsync(User user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> CheckPasswordAsync(User user, string oldPassword)
        {
            throw new NotImplementedException();
        }

        public Task CheckRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckUserInRoleAsync(User user, string roleName)
        {
            throw new NotImplementedException();
        }

        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirect)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SelectListItem> GetComboExistingRoles()
        {
            throw new NotImplementedException();
        }

        public Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdWithRoleNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameByRoleIdAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTotalUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            throw new NotImplementedException();
        }

        public Task SignInAsync(User user, bool isPersistent)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}

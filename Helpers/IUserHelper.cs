using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Garage88.Data.Entities;
using Garage88.Models;

namespace Garage88.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<APIUserViewModel> GetUserDetailsAsync(string email);

        Task<User> GetUserByIdAsync(string userId);

        Task CheckRoleAsync(string roleName);

        Task<bool> CheckUserInRoleAsync(User user, string roleName);

        Task<IdentityResult> AddUserToRoleAsync(User user, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        IEnumerable<SelectListItem> GetComboExistingRoles();

        Task<string> GetRoleNameByRoleIdAsync(string roleId);

        Task<string> GetRoleIdWithRoleNameAsync(string roleName);

        Task<List<UserDataChartModel>> GetUsersChartDataAsync();

        Task<int> GetTotalUsersAsync();

        Task<SignInResult> CheckPasswordAsync(User user, string oldPassword);


        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirect);

        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);

        Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo info);

        Task<IdentityResult> CreateAsync(User user);

        Task<IdentityResult> AddLoginAsync(User user, ExternalLoginInfo info);

        Task SignInAsync(User user, bool isPersistent);

        Task<bool> HasPasswordAsync(User user);

    }
}
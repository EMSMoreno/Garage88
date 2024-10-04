using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage88.Helpers
{
    public interface IMailHelper
    {
        Task<Response> SendEmail(string to, string subject, string body, string attachment);

        List<SelectListItem> Destinations();

        Task<Response> SendAnnouncementAsync(int to, string subject, string body, string path);

        Task<Response> SendContactEmailAsync(string email, string subject, string message, string custName);
    }
}

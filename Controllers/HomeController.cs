using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Garage88.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMailHelper _mailHelper;
        private readonly IUserHelper _userHelper;

        public HomeController(ILogger<HomeController> logger, IMailHelper mailHelper, IUserHelper userHelper)
        {
            _logger = logger;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                ViewBag.UserProfilePicture = user?.ProfilePicture ?? Guid.Empty;
                ViewBag.UserName = user?.UserName;
            }
            return View();
        }

        public IActionResult Contact()
        {
            var model = new ContactFormViewModel();
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var message = $"<p> New client contact request <br/><br/><b>Name: <b/>{model.Name}<br/><b>Phone Number: </b>{model.PhoneNumber}<br/>" +
                    $"<b>Email: </b> {model.Email}<br/><b>Plate Number: </b>{model.PlateNumber}<br/><b>Message: </b>{model.Message}<br/><br/>Please refer to this client as soon as possible. </br>" +
                    $"Garage88 Management Team.";

                var response = await _mailHelper.SendContactEmailAsync(model.Email, "Contact request", message, model.Name);

                if (response.IsSuccess)
                {
                    ViewBag.Message = "Your contact request was sent successfully! We will get in touch with you as soon as possible.";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    _logger.LogError("Failed to send contact email: " + response.Message);
                    ViewBag.Message = "There was a problem sending your contact request. Please try again.";
                    return View(model);
                }
            }

            _logger.LogWarning("Contact form validation failed.");
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogWarning(error.ErrorMessage);
            }

            return View(model);
        }

        public IActionResult CarCare() => View();
        public IActionResult Brakes() => View();
        public IActionResult Diagnostics() => View();
        public IActionResult Electrical() => View();
        public IActionResult ACService() => View();
        public IActionResult Engine() => View();
        public IActionResult Maintenance() => View();
        public IActionResult Services() => View();
        public IActionResult Electronic() => View();
        public IActionResult Undercar() => View();
    }
}

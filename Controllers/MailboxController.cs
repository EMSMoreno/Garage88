using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin, Receptionist")]
    public class MailboxController : Controller
    {
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IAppointmentRepository _appointmentRepository;

        public MailboxController(IMailHelper mailHelper, IFlashMessage flashMessage, IAppointmentRepository appointmentRepository)
        {
            _mailHelper = mailHelper;
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
        }

        public IActionResult Announcement()
        {
            var model = new AnnouncementViewModel
            {
                To = _mailHelper.Destinations()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Announcement(int ToId, string Subject, string Message)
        {
            if (ModelState.IsValid)
            {
                var result = await _mailHelper.SendAnnouncementAsync(ToId, Subject, Message, null);

                if (result.IsSuccess)
                {
                    ViewBag.SuccessMessage = "Announcement sent successfully!";
                    return RedirectToAction("Announcement");
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                }
            }

            return View();
        }

        public async Task<IActionResult> InformClientAppointment()
        {
            int emailsSent = 0;
            var tommorowAppointments = await _appointmentRepository.GetTommorowAppointmentsAsync(DateTime.UtcNow.AddDays(1));

            if (tommorowAppointments == null || !tommorowAppointments.Any())
            {
                return RedirectToAction("Index", "Appointment");
            }

            foreach (var appointment in tommorowAppointments)
            {
                if (!appointment.IsInformed)
                {
                    var response = await _mailHelper.SendEmail(
                        appointment.Client.Email,
                        "Appointment Confirmation Garage88",
                        $"<h1>Appointment Confirmation</h1>" +
                        $" Mr/Mrs {appointment.Client.FullName},<br>We send you this email to remind you that you have an appointment with our services for tomorrow ({DateTime.UtcNow.AddDays(1).ToString("dd/MM/yyyy")}) " +
                        $"at {appointment.AppointmentStartDate.ToString("HH:mm")} with your {appointment.Vehicle.Brand.Name} {appointment.Vehicle.Model.Name} ({appointment.Vehicle.PlateNumber}).<br>" +
                        $"<br>Thanks for your time and Hope to see you Tomorrow! <br> Best regards, <br> Garage88 ", null
                    );

                    if (!response.IsSuccess)
                    {
                        _flashMessage.Danger("There was an error sending the confirmation emails");
                        return RedirectToAction("Index", "Appointment");
                    }

                    try
                    {
                        appointment.IsInformed = true;
                        await _appointmentRepository.UpdateAsync(appointment);
                        emailsSent++;
                    }
                    catch (Exception)
                    {
                        _flashMessage.Danger("There was an error updating the appointment information.");
                        return RedirectToAction("Index", "Appointment");
                    }
                }
            }

            if (emailsSent == 0)
            {
                _flashMessage.Info("All the clients for tomorrow were already informed.");
            }
            else
            {
                _flashMessage.Confirmation("All the clients for tomorrow were informed!");
                return RedirectToAction("AddAppointment", "Appointment");
            }

            return RedirectToAction("Index", "Appointment");
        }

    }
}

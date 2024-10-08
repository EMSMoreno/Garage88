using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;

namespace Garage88.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IFlashMessage _flashMessage;

        public AppointmentController(IFlashMessage flashMessage, IAppointmentRepository appointmentRepository
                                    , IMechanicRepository mechanicRepository, IClientRepository clientRepository
                                    , IEstimateRepository estimateRepository, IUserHelper userHelper
                                    , IConverterHelper converterHelper, IMailHelper mailHelper)
        {
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _mechanicRepository = mechanicRepository;
            _clientRepository = clientRepository;
            _estimateRepository = estimateRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
        }

        public IActionResult Index()
        {
            //adiciona um refresh rate à ação index do controlador

            Response.Headers.Add("Refresh", "60");


            var events = _appointmentRepository.GetAllEvents();
            ViewData["Events"] = events;

            return View();
        }

        //[Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> AddAppointment(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var estimateToAppoint = await _estimateRepository.GetEstimateWithDetailsByIdAsync(id.Value);

            if (estimateToAppoint == null)
            {
                return NotFound();
            }

            if (estimateToAppoint.HasAppointment != true)
            {
                var model = new AppointmentViewModel
                {
                    Technicians = _mechanicRepository.GetComboTechnicians(),
                    ClientId = estimateToAppoint.Client.Id,
                    VehicleId = estimateToAppoint.Vehicle.Id,
                    EstimateId = estimateToAppoint.Id,
                    Client = estimateToAppoint.Client,
                    Vehicle = estimateToAppoint.Vehicle,
                };

                var events = _appointmentRepository.GetAllEvents();
                ViewData["Events"] = events;

                return View(model);
            }

            _flashMessage.Warning("The estimate that you are selecting already has an appointment made!");
            return RedirectToAction("Index", "Estimates");
        }

        //[Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAppointment(AppointmentViewModel model)
        {
            var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(model.EstimateId);

            string servicesString = "";
            foreach (var item in estimate.Services)
            {
                if (item.Service != null)
                {
                    servicesString += item.Service.Name + "<br>";
                }
            }

            if (ModelState.IsValid)
            {
                string obs = "";

                if (string.IsNullOrEmpty(model.Observations))
                {
                    obs = "No observations added.";
                }
                else
                {
                    obs = model.Observations;
                }

                var appointment = new Appointment
                {
                    Observations = obs,
                    Mechanic = await _mechanicRepository.GetMechanicByIdAsync(model.MechanicId),
                    Client = estimate.Client,
                    Vehicle = estimate.Vehicle,
                    Estimate = estimate,
                    CreatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                    UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                    AppointmentStartDate = model.AppointmentStartDate,
                    AppointmentEndDate = model.AppointmentEndDate,
                    CreatedDate = model.CreatedDate,
                    UpdatedDate = model.UpdatedDate,
                    AppointmentServicesDetails = servicesString,
                };

                try
                {
                    await _appointmentRepository.CreateAsync(appointment);
                    estimate.HasAppointment = true;
                    await _estimateRepository.UpdateAsync(estimate);
                    _flashMessage.Confirmation("The appointment was created with success!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was a problem creating the appointment! " + ex.InnerException);
                    model.Client = estimate.Client;
                    model.Vehicle = estimate.Vehicle;
                    model.Technicians = _mechanicRepository.GetComboTechnicians();
                    return View(model);
                }
            }

            ViewData["Events"] = _appointmentRepository.GetAllEvents();
            model.Client = estimate.Client;
            model.Vehicle = estimate.Vehicle;
            model.Technicians = _mechanicRepository.GetComboTechnicians();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id.Value);

            if (appointment == null)
            {
                return NotFound();
            }

            //if(appointment.AppointmentStartDate < DateTime.UtcNow)
            //{
            //    _flashMessage.Warning("It is not possible to edit an appointment from the past.");
            //    return RedirectToAction(nameof(Index));
            //}

            var model = _converterHelper.ToAppointmentViewModel(appointment, false);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppointmentViewModel model)
        {
            //Devido à restrição imposta para que as marcações sejam todas procedidas para depois da data e hora atual, não permitia editar marcações do passado apesar de
            //dados se encontrarem preenchidos no modelo.

            if (ModelState.ErrorCount < 2 && !string.IsNullOrEmpty(model.AppointmentStartDate.ToString()))
            {
                var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(model.EstimateId);

                string obs = "";

                if (string.IsNullOrEmpty(model.Observations))
                {
                    obs = "No observations added.";
                }
                else
                {
                    obs = model.Observations;
                }

                var appointment = await _appointmentRepository.GetAppointmentByIdAsync(model.Id);

                if (appointment == null)
                {
                    _flashMessage.Danger("There was an error updating the appointment");
                    return RedirectToAction(nameof(Index));
                }

                appointment.Observations = obs;
                appointment.UpdatedDate = DateTime.Now;
                appointment.Mechanic = await _mechanicRepository.GetMechanicByIdAsync(model.MechanicId);
                appointment.UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                try
                {
                    await _appointmentRepository.UpdateAsync(appointment);
                    _flashMessage.Confirmation("Your appointment was updated with Success!");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("There was an error updating your appointment " + ex.Message);
                    model = _converterHelper.ToAppointmentViewModel(appointment, false);
                    return View(model);
                }
            }

            _flashMessage.Danger("Failed to update your appointment.");
            return RedirectToAction(nameof(Index));

        }

        //[Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [Route("Appointment/EventResize")]
        public async Task<bool> EventResize(int eventId, string startTime, string endTime)
        {
            if (eventId == 0 || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            {
                return false;
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(eventId);

            if (appointment == null)
            {
                return false;
            }

            var startDateTime = Convert.ToDateTime(startTime).ToUniversalTime();
            var endDateTime = Convert.ToDateTime(endTime).ToUniversalTime();

            appointment.AppointmentEndDate = endDateTime;
            appointment.AppointmentStartDate = startDateTime;
            appointment.UpdatedDate = DateTime.Now;
            appointment.UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            try
            {
                await _appointmentRepository.UpdateAsync(appointment);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //[Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id.Value);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.AsAttended == true)
            {
                _flashMessage.Warning($"It is not possible to cancel the appointment. The appointment already has a work order going or done.");
                return RedirectToAction(nameof(Index));
            }

            var clientFullName = appointment.Client.FullName;
            var estimate = appointment.Estimate;
            var email = appointment.Client.Email;
            var appointmentDate = appointment.AppointmentStartDate.ToString("dd/MM/yyyy");
            var appointmentHour = appointment.AppointmentStartDate.ToString("HH:mm");
            var vehicle = appointment.Vehicle;

            try
            {
                await _appointmentRepository.DeleteAsync(appointment);

                estimate.HasAppointment = false;
                await _estimateRepository.UpdateAsync(estimate);

                await _mailHelper.SendEmail(email, "Appointment Cancelation Garage88", $"<h1>Appointment Cancelation</h1>" +
                    $" Mr/Mrs {clientFullName},<br>We send you this email to inform you that the appointment you had with our services for {appointmentDate} " +
                    $"at {appointmentHour} with your {vehicle.Brand.Name} {vehicle.Model.Name} ({vehicle.PlateNumber}) was canceled.<br> For more information, please contact us by either replying to this email or" +
                    $" to our contact number: 216589564. <br>" +
                    $"<br>Thank you for your time and hope to see you soon! <br> Best regards, <br> Garage88 ", null);

                _flashMessage.Warning($"The appointment from {clientFullName} was canceled with Success.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Warning($"There was a problem canceling the appointment. {ex.InnerException}.");
                return RedirectToAction(nameof(Index));
            }

        }
    }
}

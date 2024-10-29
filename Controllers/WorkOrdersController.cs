using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Vereyon.Web;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin, Technician, Receptionist")]
    public class WorkOrdersController : Controller
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserHelper _userHelper;
        private readonly IMechanicRepository _mechanicRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IJSRuntime _iJSRuntime;

        public WorkOrdersController(IWorkOrderRepository workOrderRepository, IFlashMessage flashMessage, IAppointmentRepository appointmentRepository, IUserHelper userHelper, IMechanicRepository mechanicRepository, IInvoiceRepository invoiceRepository, IJSRuntime iJSRuntime)
        {
            _workOrderRepository = workOrderRepository;
            _flashMessage = flashMessage;
            _appointmentRepository = appointmentRepository;
            _userHelper = userHelper;
            _mechanicRepository = mechanicRepository;
            _invoiceRepository = invoiceRepository;
            _iJSRuntime = iJSRuntime;

        }

        public IActionResult Index()
        {
            var workOrders = _workOrderRepository.GetAllWorkOrders();

            return View(workOrders);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [Route("WorkOrders/Create")]
        public async Task<int> Create(int? id)
        {
            if (id == null)
            {
                return 0;
            }

            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id.Value);

            if (appointment == null)
            {
                return 0;
            }

            var workOrder = new WorkOrder
            {
                Appointment = appointment,
                CreatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                IsFinished = false,
                OrderDateStart = DateTime.UtcNow,
                Status = "Opened",
            };

            try
            {
                await _workOrderRepository.CreateAsync(workOrder);
                _flashMessage.Info($"{appointment.Client.FullName} work order was successfuly created.");
                appointment.AsAttended = true;
                await _appointmentRepository.UpdateAsync(appointment);
                return appointment.Id;
            }
            catch (Exception ex)
            {
                _flashMessage.Info($"There was an error creating {appointment.Client.FullName} work order. {ex.InnerException}.");
                return 0;
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            return RedirectToAction("Edit", "Estimates", new { id = workOrder.Appointment.Estimate.Id, isNew = true });
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            return View(workOrder);

        }

        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> DeclareDone(int? id, string observations, int mechanicId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            if (workOrder.IsFinished != true)
            {
                var mechanic = await _mechanicRepository.GetMechanicByIdAsync(mechanicId);

                if (mechanic == null)
                {
                    return NotFound();
                }

                workOrder.ServiceDoneBy = mechanic.User;
                workOrder.Observations = observations;
                workOrder.IsFinished = true;
                workOrder.awaitsReceipt = true;
                workOrder.Status = "Done";
                workOrder.Appointment.Mechanic = mechanic;

                try
                {
                    await _workOrderRepository.UpdateAsync(workOrder);
                    _flashMessage.Confirmation($"Work order nº{workOrder.Id} as been declared done.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    _flashMessage.Warning($"There was a problem declaring work order as done.");
                    return RedirectToAction(nameof(Index));
                }
            }

            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            if (workOrder.Status != "Opened")
            {
                _flashMessage.Warning("It is not possible to delete/Cancel a work order that has already been processed.");
                return RedirectToAction(nameof(Index));
            }

            workOrder.Appointment.AsAttended = false;

            try
            {
                await _appointmentRepository.UpdateAsync(workOrder.Appointment);
                await _workOrderRepository.DeleteAsync(workOrder);
                _flashMessage.Warning($"Work order nº{workOrder.Id} as been canceled.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Warning($"There was an error canceling the order. {ex.InnerException}");
                return RedirectToAction(nameof(Index));
            }

        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [Route("WorkOrders/PrintInvoice")]
        public async Task<int> PrintInvoice(int? id)
        {
            if (id == null)
            {
                return 0;
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(id.Value);

            if (workOrder == null)
            {
                return 0;
            }

            if (workOrder.Status == "Done" && workOrder.IsFinished == true)
            {
                var invoice = new Invoice
                {
                    CreatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name),
                    Client = workOrder.Appointment.Client,
                    Estimate = workOrder.Appointment.Estimate,
                    Vehicle = workOrder.Appointment.Vehicle,
                    WorkOrder = workOrder,
                    InvoicDate = DateTime.UtcNow,
                    Value = Convert.ToDecimal(workOrder.Appointment.Estimate.ValueWithDiscount),
                };

                try
                {
                    await _invoiceRepository.CreateAsync(invoice);
                    workOrder.Status = "Closed";
                    workOrder.OrderDateEnd = DateTime.UtcNow;
                    workOrder.UpdatedBy = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                    workOrder.awaitsReceipt = false;
                    await _workOrderRepository.UpdateAsync(workOrder);
                    var recentInvoice = await _invoiceRepository.GetRecentCreatedInvoiceAsync(workOrder.Id);
                    _flashMessage.Confirmation("The invoice was created with success.");
                    return recentInvoice.Id;
                }
                catch (Exception ex)
                {
                    _flashMessage.Confirmation($"There was a problem creating the invoice. {ex.InnerException}");
                    return 0;
                }
            }

            else
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("WorkOrders/CheckValidMechanicId")]
        public async Task<JsonResult> CheckValidMechanicId(int mechanicId)
        {
            var mechanic = await _mechanicRepository.GetMechanicByIdAsync(mechanicId);
            var json = Json(mechanic);
            return json;
        }

        [HttpPost]
        [Route("WorkOrders/GetWorkOrder")]
        public async Task<JsonResult> GetWorkOrder(int workOrderId)
        {
            var workOrder = await _workOrderRepository.GetWorkOrderByIdAsync(workOrderId);
            var json = Json(workOrder);
            return json;
        }
    }
}

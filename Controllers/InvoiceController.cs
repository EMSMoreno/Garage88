using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Repositories;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin, Receptionist")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IWorkOrderRepository _workOrderRepository;


        public InvoiceController(IInvoiceRepository invoiceRepository,
            IWorkOrderRepository workOrderRepository)
        {
            _invoiceRepository = invoiceRepository;
            _workOrderRepository = workOrderRepository;
        }

        public IActionResult Index()
        {
            var invoices = _invoiceRepository.GetAllInvoices();

            return View(invoices);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetInvoiceDetailsByIdAsync(id.Value);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }
    }
}

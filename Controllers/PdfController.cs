using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Vereyon.Web;
using SelectPdf;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Garage88.Controllers
{
    public class PdfController : Controller
    {
        private readonly ICompositeViewEngine _compositeViewEngine;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMailHelper _mailHelper;
        private readonly IEstimateRepository _estimateRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IWorkOrderRepository _workOrderRepository;

        public PdfController(ICompositeViewEngine compositeViewEngine,
            IInvoiceRepository invoiceRepository,
            IMailHelper mailHelper,
            IEstimateRepository estimateRepository,
            IFlashMessage flashMessage,
            IWorkOrderRepository workOrderRepository
        )
        {
            _compositeViewEngine = compositeViewEngine;
            _invoiceRepository = invoiceRepository;
            _mailHelper = mailHelper;
            _estimateRepository = estimateRepository;
            _flashMessage = flashMessage;
            _workOrderRepository = workOrderRepository;
            GlobalProperties.EnableRestrictedRenderingEngine = true;
            GlobalProperties.EnableFallbackToRestrictedRenderingEngine = true;
        }

        public async Task<IActionResult> Invoice(int? id, bool sendEmail)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetInvoiceDetailsByIdAsync(id.Value);

            using (var stringWriter = new StringWriter())
            {

                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_Invoice", false);

                if (viewResult == null)
                {
                    throw new ArgumentNullException("View cannot be found");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewDictionary.Model = invoice;

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                    );

                await viewResult.View.RenderAsync(viewContext);

                var htmlToPdf = new HtmlToPdf(1000, 1414);
                htmlToPdf.Options.DrawBackground = true;


                var pdf = htmlToPdf.ConvertHtmlString(stringWriter.ToString());
                var pdfBytes = pdf.Save();

                if (sendEmail == true)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\tempData\invoice" + invoice.Id + ".pdf");
                    FileStream fs = new FileStream(path, FileMode.Create);

                    using (var streamWriter = new StreamWriter(fs))
                    {
                        await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                    }

                    Response response = await _mailHelper.SendEmail(invoice.Client.Email, "Invoice Garage88", $"<h1>Service Invoice</h1>" +
                        $" Mr/Mrs {invoice.Client.FullName},<br>you can find attached to this email the invoice corresponding to the service done on " +
                        $"your {invoice.Vehicle.Brand.Name} {invoice.Vehicle.Model.Name} with the following plate number: {invoice.Vehicle.PlateNumber} .<br>" +
                        $"<br> Thanks for trusting in our services! <br> Best regards, <br> Garage88 ", path);

                    if (response.IsSuccess == true)
                    {
                        System.IO.File.Delete(path);
                    }
                }

                return File(pdfBytes, "application/pdf");

            }
        }

        public async Task<IActionResult> Estimate(int? id, bool sendEmail)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estimate = await _estimateRepository.GetEstimateWithDetailsByIdAsync(id.Value);

            using (var stringWriter = new StringWriter())
            {
                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_Estimate", false);

                if (viewResult == null)
                {
                    throw new ArgumentNullException("View cannot be found");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewDictionary.Model = estimate;

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                    );

                await viewResult.View.RenderAsync(viewContext);

                var htmlToPdf = new HtmlToPdf(1000, 1414);
                htmlToPdf.Options.DrawBackground = true;

                var pdf = htmlToPdf.ConvertHtmlString(stringWriter.ToString());
                var pdfBytes = pdf.Save();

                if (sendEmail == true)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\tempData\invoice" + estimate.Id + ".pdf");
                    FileStream fs = new FileStream(path, FileMode.Create);

                    using (var streamWriter = new StreamWriter(fs))
                    {
                        await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                    }

                    Response response = await _mailHelper.SendEmail(estimate.Client.Email, "Estimate Garage88", $"<h1>Estimate</h1>" +
                        $" Mr/Mrs {estimate.Client.FullName},<br>you can find attached to this email the estimate corresponding to the service done on " +
                        $"your {estimate.Vehicle.Brand.Name} {estimate.Vehicle.Model.Name} with the following plate number: {estimate.Vehicle.PlateNumber} .<br>" +
                        $"<br> Thanks for trusting in our services! And hope to seen you soon! <br> Best regards, <br> Garage88 ", path);

                    if (response.IsSuccess == true)
                    {
                        _flashMessage.Confirmation($"Estimate was succefully sent to: {estimate.Client.Email}.");
                        System.IO.File.Delete(path);
                        return RedirectToAction("Details", "Estimates", new { id = estimate.Id });
                    }
                    else
                    {
                        _flashMessage.Danger($"there was a problem sending the email to: {estimate.Client.Email}.");
                        System.IO.File.Delete(path);
                        return RedirectToAction("Details", "Estimates", new { id = estimate.Id });
                    }
                }

                return File(pdfBytes, "application/pdf");

            }
        }

        public async Task<IActionResult> WorkOrder(int? id)
        {

            if (id == 0)
            {
                return NotFound();
            }

            var workOrder = await _workOrderRepository.GetWorkOrderByAppointmentIdAsync(id.Value);

            if (workOrder == null)
            {
                return NotFound();
            }

            using (var stringWriter = new StringWriter())
            {
                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_WorkOrder", false);

                if (viewResult == null)
                {
                    throw new ArgumentNullException("View cannot be found");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
                viewDictionary.Model = workOrder;

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    stringWriter,
                    new HtmlHelperOptions()
                    );

                await viewResult.View.RenderAsync(viewContext);

                var htmlToPdf = new HtmlToPdf(1000, 1414);
                htmlToPdf.Options.DrawBackground = true;

                var pdf = htmlToPdf.ConvertHtmlString(stringWriter.ToString());
                var pdfBytes = pdf.Save();

                return File(pdfBytes, "application/pdf");

            }
        }
    }
}

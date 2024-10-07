using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Garage88.Data.Repositories;
using Garage88.Helpers;

namespace Garage88.Controllers
{
    [Authorize(Roles = "Admin, Technician, Receptionist")]
    public class DashboardPanelController : Controller
    {
        private readonly IWorkOrderRepository _workOrderRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserHelper _userHelper;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;

        public DashboardPanelController(IWorkOrderRepository workOrderRepository
            , IInvoiceRepository invoiceRepository
            , IUserHelper userHelper
            , IVehicleRepository vehicleRepository
            , IServiceRepository serviceRepository)
        {
            _workOrderRepository = workOrderRepository;
            _invoiceRepository = invoiceRepository;
            _userHelper = userHelper;
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<IActionResult> Index()
        {
            //this month sales graphic data
            var thisMonthsales = await _invoiceRepository.GetMonthlySales(DateTime.UtcNow.Month);

            ViewBag.MonthlySales = thisMonthsales;
            ViewBag.MonthName = thisMonthsales.FirstOrDefault().Month;
            string[] color1 = { "#efcfe3" };
            string[] color2 = { "#ea9ab2" };
            string[] color3 = { "#e27396" };
            ViewBag.color1 = color1;
            ViewBag.color2 = color2;
            ViewBag.color3 = color3;

            //all months overal sales graphic data
            var overallMonthsSales = await _invoiceRepository.GetYearSalesByMonthAsync(DateTime.UtcNow.Year);
            ViewBag.OverallMonthsSales = overallMonthsSales;
            //total users data
            ViewBag.totalUsers = await _userHelper.GetTotalUsersAsync();
            //active work orders data
            ViewBag.activeWorkOrders = await _workOrderRepository.GetActiveWorkOrdersNumber();
            //Total Registered Vehicles
            ViewBag.registeredVehicles = await _vehicleRepository.GetAllRegisteredVehiclesNumberAsync();
            //Most sold Service
            var services = await _serviceRepository.GetMostSoldServicesData();
            ViewBag.mostSoldService = services.Last().Name;

            return View();
        }

        [HttpPost]
        [Route("DashboardPanel/GetOpenedWorkOrders")]
        public async Task<JsonResult> GetOpenedWorkOrders()
        {
            var OpenedWorkOrders = await _workOrderRepository.GetOpenedWorkOrdersAsync();
            var json = Json(OpenedWorkOrders);
            return json;
        }

        [HttpPost]
        [Route("DashboardPanel/GetChartUserData")]
        public async Task<JsonResult> GetChartUserData()
        {
            var registeredUserData = await _userHelper.GetUsersChartDataAsync();
            var json = Json(registeredUserData);
            return json;
        }

        [HttpPost]
        [Route("DashboardPanel/GetWorkOrdersChart")]
        public async Task<JsonResult> GetWorkOrdersChart()
        {
            var workOrdersData = await _workOrderRepository.GetWorkOrdersChartAsync(DateTime.UtcNow.Month);
            var json = Json(workOrdersData);
            return json;
        }

        [HttpPost]
        [Route("DashboardPanel/GetVehiclesChartData")]
        public async Task<JsonResult> GetVehiclesChartData()
        {
            var vehiclesData = await _vehicleRepository.GetVehiclesChartDataAsync();
            var json = Json(vehiclesData);
            return json;
        }


        [HttpPost]
        [Route("DashboardPanel/GetServicesChartData")]
        public async Task<JsonResult> GetServicesChartData()
        {
            var servicesData = await _serviceRepository.GetMostSoldServicesData();
            var json = Json(servicesData);
            return json;
        }
    }
}

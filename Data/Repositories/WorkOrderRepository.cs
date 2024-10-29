using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.EntityFrameworkCore;

namespace Garage88.Data.Repositories
{
    public class WorkOrderRepository : GenericRepository<WorkOrder>, IWorkOrderRepository
    {
        private readonly DataContext _context;

        public WorkOrderRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetActiveWorkOrdersNumber()
        {
            var workOrders = await _context.WorkOrders.Where(wo => wo.Status != "Closed").ToListAsync();

            return workOrders.Count;
        }

        public IQueryable GetAllWorkOrders()
        {
            return _context.WorkOrders
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Client)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model);
        }

        public async Task<List<APIServiceViewModel>> GetAllWorkOrdersByPlateNumberAsync(string plateNumber)
        {
            List<APIServiceViewModel> list = new List<APIServiceViewModel>();

            var vehicle = _context.Vehicles
                 .Include(v => v.Brand)
                 .Include(v => v.Model)
                 .Where(v => v.PlateNumber == plateNumber).FirstOrDefault();


            var workOrders = await _context.WorkOrders
                 .Include(wo => wo.Appointment)
                     .ThenInclude(a => a.Vehicle)
                  .Include(wo => wo.Appointment)
                     .ThenInclude(a => a.Mechanic)
                 .Where(wo => wo.Appointment.Vehicle.Id == vehicle.Id).ToListAsync();

            foreach (var workOrder in workOrders)
            {
                list.Add(new APIServiceViewModel
                {
                    Id = workOrder.Id,
                    PlateNumber = vehicle.PlateNumber,
                    Brand = vehicle.Brand.Name,
                    Model = vehicle.Model.Name,
                    Vin = vehicle.VehicleIdentificationNumber,
                    ServiceDateStart = workOrder.OrderDateStart,
                    ServiceDateEnd = workOrder.OrderDateEnd,
                    IsFinished = workOrder.IsFinished,
                    Observations = workOrder.Observations,
                    Status = workOrder.Status,
                    ServiceDoneBy = workOrder.Appointment.Mechanic.FullName
                });
            }

            return list;
        }

        public async Task<int> GetOpenedWorkOrdersAsync()
        {
            var workOrders = _context.WorkOrders.Where(wo => wo.Status == "Opened");
            return workOrders.Count();
        }

        public async Task<WorkOrder> GetWorkOrderByAppointmentIdAsync(int appointmentId)
        {
            return await _context.WorkOrders
                .Include(wo => wo.ServiceDoneBy)
                .Include(wo => wo.UpdatedBy)
                .Include(wo => wo.CreatedBy)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Client)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Mechanic)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Estimate)
                        .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Brand)
                .Include(wo => wo.Appointment)
                    .ThenInclude(a => a.Vehicle)
                        .ThenInclude(v => v.Model)
                .Where(wo => wo.Appointment.Id == appointmentId).FirstOrDefaultAsync();
        }

        public async Task<WorkOrder> GetWorkOrderByIdAsync(int id)
        {
            return await _context.WorkOrders
            .Include(w => w.Appointment) 
            .Include(w => w.ServiceDoneBy)
            .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<WorkOrderChartDataModel>> GetWorkOrdersChartAsync(int month)
        {
            List<WorkOrderChartDataModel> list = new List<WorkOrderChartDataModel>();
            string[] status = new string[3] { "Closed", "Opened", "Done" };
            string[] color = new string[3] { "#990000", "#FFA500", "#9EB23B" };
            int id = 0;

            foreach (string statusItem in status)
            {
                var workOrders = await _context.WorkOrders.Where(wo => wo.Status == statusItem && wo.OrderDateStart.Month == month && wo.OrderDateStart.Year == DateTime.UtcNow.Year).ToListAsync();

                list.Add(new WorkOrderChartDataModel
                {
                    Status = statusItem,
                    Quantity = workOrders.Count(),
                    Color = color[id]
                });

                id++;
            }

            return list;
        }
    }
}

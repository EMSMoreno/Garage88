using Garage88.Data.Entities;
using Garage88.Models;

namespace Garage88.Data.Repositories
{
    public interface IWorkOrderRepository : IGenericRepository<WorkOrder>
    {
        IQueryable GetAllWorkOrders();

        Task<WorkOrder> GetWorkOrderByIdAsync(int id);

        Task<int> GetOpenedWorkOrdersAsync();
        Task<WorkOrder> GetWorkOrderByAppointmentIdAsync(int appointmentId);

        Task<List<WorkOrderChartDataModel>> GetWorkOrdersChartAsync(int month);
        Task<int> GetActiveWorkOrdersNumber();

        Task<List<APIServiceViewModel>> GetAllWorkOrdersByPlateNumberAsync(string plateNumber);
        Task CreateAsync(WorkOrder workOrder);
    }
}

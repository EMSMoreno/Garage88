using Garage88.Data.Entities;
using Garage88.Models;

namespace Garage88.Data.Repositories
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        IQueryable GetAllInvoices();

        Task<Invoice> GetInvoiceDetailsByIdAsync(int id);

        Task<Invoice> GetRecentCreatedInvoiceAsync(int workOrderId);

        Task<List<SalesChartDataModel>> GetMonthlySales(int month);

        Task<List<SalesChartDataModel>> GetYearSalesByMonthAsync(int year);

        Task<List<Invoice>> GetUserInvoicesAsync(int customerId);
    }
}

using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Garage88.Data.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly DataContext _context;

        public InvoiceRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllInvoices()
        {
            return _context.Invoices
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)
                .Include(i => i.Client)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model);
        }

        public async Task<Invoice> GetInvoiceDetailsByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.Appointment)
                .Include(i => i.Client)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                    .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model)
                .Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<SalesChartDataModel>> GetMonthlySales(int month)
        {
            DateTime requestedDate = new DateTime(DateTime.UtcNow.Year, month, DateTime.UtcNow.Day);
            List<SalesChartDataModel> list = new List<SalesChartDataModel>();

            double value;
            CultureInfo ci = new CultureInfo("en-Us");
            int days = DateTime.DaysInMonth(DateTime.UtcNow.Year, month);

            for (int day = 1; day <= days; day++)
            {
                value = (double)await _context.Invoices.Where(i => i.InvoicDate.Day == day && i.InvoicDate.Month == requestedDate.Month).SumAsync(i => i.Value);

                list.Add(new SalesChartDataModel
                {
                    Month = requestedDate.ToString("MMMM", ci).ToUpper(),
                    Day = day,
                    Sales = value
                });
            }

            return list;
        }

        public async Task<Invoice> GetRecentCreatedInvoiceAsync(int workOrderId)
        {
            return await _context.Invoices
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.Appointment)
                .Include(i => i.Client)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                    .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model)
                .Where(i => i.WorkOrder.Id == workOrderId).FirstOrDefaultAsync();
        }

        public async Task<List<Invoice>> GetUserInvoicesAsync(int clientId)
        {
            return await _context.Invoices
                   .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.ServiceDoneBy)
                .Include(i => i.WorkOrder)
                    .ThenInclude(wo => wo.Appointment)
                .Include(i => i.Client)
                .Include(i => i.CreatedBy)
                .Include(i => i.Estimate)
                    .ThenInclude(e => e.Services)
                            .ThenInclude(es => es.Service)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Brand)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Model)
                .Where(i => i.Client.Id == clientId).ToListAsync();
        }

        public async Task<List<SalesChartDataModel>> GetYearSalesByMonthAsync(int year)
        {
            DateTime requestedYearDate = new DateTime(year, 1, 1);

            List<SalesChartDataModel> list = new List<SalesChartDataModel>();
            double value;
            CultureInfo ci = new CultureInfo("en-US");

            for (int month = 1; month <= 12; month++)
            {
                value = (double)await _context.Invoices
                    .Where(i => i.InvoicDate.Month == month && i.InvoicDate.Year == requestedYearDate.Year)
                    .SumAsync(i => i.Value);

                list.Add(new SalesChartDataModel
                {
                    Year = year.ToString(),
                    Month = new DateTime(year, month, 1).ToString("MMMM", ci),
                    Sales = value,
                });
            }

            return list;

        }
    }
}

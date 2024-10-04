using Garage88.Data.Entities;
using Garage88.Helpers;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Garage88.Data.Repositories
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public ServiceRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task AddServiceToEstimateAsync(AddServiceToEstimateViewModel model, string userName)
        {

            var user = await _userHelper.GetUserByEmailAsync(userName);

            if (user == null)
            {
                return;
            }

            var service = await _context.Services.FindAsync(model.ServiceId);

            if (service == null)
            {
                return;
            }

            var estimateDetailTemp = await _context.EstimateDetailTemps.Where(edt => edt.VehicleId == model.VehicleId && edt.Service == service && edt.ClientId == model.ClientId).FirstOrDefaultAsync();

            if (estimateDetailTemp == null)
            {
                estimateDetailTemp = new EstimateDetailTemp
                {
                    Price = service.Price,
                    Service = service,
                    Quantity = model.Quantity,
                    User = user,
                    ClientId = model.ClientId,
                    VehicleId = model.VehicleId,
                    EstimateId = model.EstimateId,
                };

                _context.EstimateDetailTemps.Add(estimateDetailTemp);
            }
            else
            {
                estimateDetailTemp.Quantity += model.Quantity;
                _context.EstimateDetailTemps.Update(estimateDetailTemp);
            }

            await _context.SaveChangesAsync();
        }

        public IEnumerable<SelectListItem> GetComboServices()
        {
            var list = _context.Services.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString(),
            }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Select a service]",
                Value = "0"
            });

            return list;
        }

        public async Task<List<ServiceChartModel>> GetMostSoldServicesData()
        {
            List<ServiceChartModel> list = new List<ServiceChartModel>();
            Random r = new Random();

            var thisMonthInvoices = await _context.Invoices
                .Include(i => i.Estimate)
                .ThenInclude(e => e.Services)
                .ThenInclude(s => s.Service)
                .Where(i => i.InvoicDate.Month == DateTime.UtcNow.Month)
                .ToListAsync();


            var services = await _context.Services.ToListAsync();



            foreach (var service in services)
            {
                int quantity = 0;

                foreach (var invoice in thisMonthInvoices)
                {
                    foreach (var srv in invoice.Estimate.Services)
                    {
                        if (srv.Service != null)
                        {
                            if (srv.Service.Name == service.Name)
                            {
                                quantity++;
                            }
                        }
                    }
                }

                list.Add(new ServiceChartModel
                {
                    Name = service.Name,
                    Quantity = quantity,
                    Color = String.Format("#{0:X6}", r.Next(0x1000000)),
                });
            }

            return list.OrderBy(l => l.Quantity).Take(10).ToList();
        }
    }
}

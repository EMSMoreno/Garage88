using Garage88.Data.Entities;
using Garage88.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace Garage88.Data.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly DataContext _context;

        public VehicleRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetAllRegisteredVehiclesNumberAsync()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return vehicles.Count;
        }

        public IQueryable GetAllWithClients()
        {
            return _context.Vehicles.Include(v => v.Client)
                                    .Include(m => m.Model)
                                    .Include(b => b.Brand)
                                    .OrderBy(v => v.Brand.Name);
        }

        public IEnumerable<SelectListItem> GetComboVehicles(int clientId)
        {
            var customer = _context.Clients.Find(clientId);
            var list = new List<SelectListItem>();

            if (customer != null)
            {
                list = _context.Vehicles.Where(v => v.ClientId == customer.Id).Select(l => new SelectListItem
                {
                    Text = $"{l.PlateNumber}:{l.Brand.Name} - {l.Model.Name}",
                    Value = l.Id.ToString()
                }).OrderBy(p => p.Value).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "[Select a Vehicle]",
                    Value = "0"
                });
            }

            return list;
        }

        public IQueryable GetClientVehiclesAsync(int clientId)
        {
            return _context.Vehicles.Include(v => v.Client)
                                          .Include(b => b.Brand)
                                          .Include(m => m.Model)
                                          .Where(v => v.ClientId == clientId);


        }

        public async Task<Vehicle> GetNewlyAddedVehicleAsync(int id)
        {
            return await _context.Vehicles.Include(b => b.Brand).Include(v => v.Model).Where(v => v.ClientId == id).FirstOrDefaultAsync();
        }

        public async Task<Vehicle> GetVehicleDetailsByIdAsync(int id)
        {
            return await _context.Vehicles.Include(v => v.Client)
                                          .Include(b => b.Brand)
                                          .Include(m => m.Model)
                                          .Where(v => v.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<VehicleChartModel>> GetVehiclesChartDataAsync()
        {
            List<VehicleChartModel> list = new List<VehicleChartModel>();

            var vehicles = await _context.Vehicles.Include(v => v.Brand).Include(v => v.Model).ToListAsync();
            var brands = await _context.Brands.ToListAsync();
            Random r = new Random();
            var vehiclesTotal = vehicles.Count;

            foreach (var brand in brands)
            {
                var quantity = 0;

                foreach (var vehicle in vehicles)
                {
                    if (vehicle.Brand.Name == brand.Name)
                    {
                        quantity++;
                    }
                }

                var percentage = ((double)quantity / (double)vehiclesTotal).ToString("p1");

                list.Add(new VehicleChartModel
                {
                    Brand = brand.Name,
                    Quantity = quantity,
                    Color = String.Format("#{0:X6}", r.Next(0x1000000)),
                    TotalVehicles = vehicles.Count,
                    Percentage = percentage,
                });
            }

            return list;
        }
    }
}

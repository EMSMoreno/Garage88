using Microsoft.EntityFrameworkCore;
using Garage88.Helpers;
using Garage88.Data;
using Garage88.Data.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Identity;

namespace Garage88.Data
{
    public class SeedDb
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userHelper = userHelper;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Verify if data exists
            if (_context.Clients.Any() || _context.Mechanics.Any() || _context.Vehicles.Any() ||
                _context.Brands.Any() || _context.Models.Any() || _context.Appointments.Any() ||
                _context.Specialities.Any() || _context.Services.Any())
            {
                Console.WriteLine("Seed not needed, there are already data available!");
                return;
            }

            // Add especiality
            var speciality = new Speciality { Name = "Change Tires" };
            var speciality2 = new Speciality { Name = "Engine Repair" };
            await _context.Specialities.AddRangeAsync(speciality, speciality2);
            await _context.SaveChangesAsync();

            // Add role
            var role = new Role { Name = "Mechanic" };
            await _context.MechanicsRoles.AddAsync(role);
            await _context.SaveChangesAsync();

            // add mechanics
            var mechanics = new List<Mechanic>
            {
                new Mechanic { FirstName = "Mecânico", LastName = "Zézinho", Email = "zezinho@garage88.pt", About = "Mechanic with 4y of experience.", SpecialityId = speciality.Id, RoleId = role.Id },
                new Mechanic { FirstName = "Mecânico", LastName = "Maria", Email = "maria@garage88.pt", About = "Mechanic with 5y of experience.", SpecialityId = speciality.Id, RoleId = role.Id },
                new Mechanic { FirstName = "Mecânico", LastName = "Joaquim", Email = "joaquim@garage88.pt", About = "Mechanic with 6y of experience.", SpecialityId = speciality2.Id, RoleId = role.Id },
                new Mechanic { FirstName = "Mecânico", LastName = "Ana", Email = "ana@garage88.pt", About = "Mechanic with 7y of experience.", SpecialityId = speciality2.Id, RoleId = role.Id }
            };

            await _context.Mechanics.AddRangeAsync(mechanics);
            await _context.SaveChangesAsync();

            // Add client
            var client = new Client
            {
                FirstName = "Carmelita",
                LastName = "Rodrigues",
                Email = "carmelita@cinel.pt",
                Nif = "232212242",
                Address = "Rua de Exemplo, 123",
                PhoneNumber = "930008999"
            };
            _context.Clients.Add(client);

            // Add brand and model
            var brand = new Brand { Name = "Mazda" };
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            var model = new Model { Name = "Miata MX-5", BrandId = brand.Id };
            _context.Models.Add(model);

            // Add vehicle and make association w/client
            var vehicle = new Vehicle
            {
                PlateNumber = "AA-01-Z9",
                ModelId = model.Id,   // association w/ model
                ClientId = client.Id  // association w/client
            };
            _context.Vehicles.Add(vehicle);

            // Adiciona role e mecânico
            var mechanicRole = new Role { Name = "Mechanic" };
            _context.MechanicsRoles.Add(mechanicRole);
            await _context.SaveChangesAsync();

            var mechanic = new Mechanic
            {
                FirstName = "Mecânico",
                LastName = "Zézinho",
                Email = "zezinho@garage88.pt",
                About = "Mechanic with 4 years of experience.",
                SpecialityId = speciality.Id, // association w/speciality
                RoleId = mechanicRole.Id      // association w/role
            };
            _context.Mechanics.Add(mechanic);

            // Add services
            var service = new Service { Name = "Change Tires & Replace Windows" };
            _context.Services.Add(service);

            // Add appointment and association w/client
            var appointment = new Appointment
            {
                Observations = "Appointment 1",
                VehicleId = vehicle.Id,  // association w/vehicle
                ClientId = client.Id     // association w/client
            };
            _context.Appointments.Add(appointment);

            // Database Save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

            Console.WriteLine("Seed successfully!");
        }

        public async Task SeedRolesAsync()
        {
            string[] roleNames = { "Admin", "Client", "Employee" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task AddInvoiceAsync()
        {
            if (!_context.Invoices.Any())
            {
                var workOrder = await _context.WorkOrders.FirstOrDefaultAsync();
                var mechanic = await _userHelper.GetUserByEmailAsync("eduardo.sousa.moreno@formandos.cinel.pt");
                var client = await _context.Clients.Where(c => c.Email == "eduardo.sousa.moreno@sapo.pt").FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("eduardo_projeto_mecanicos@sapo.pt");
                var estimate = await _context.Estimates.FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "AA-TO-01").FirstOrDefaultAsync();

                workOrder.Status = "Closed";
                workOrder.Observations = "It is highly recommended for the client to align direction next time he comes to the shop";
                workOrder.ServiceDoneBy = mechanic;
                workOrder.IsFinished = true;

                _context.WorkOrders.Update(workOrder);

                _context.Invoices.Add(new Invoice
                {
                    Client = client,
                    CreatedBy = receptionist,
                    Estimate = estimate,
                    InvoicDate = DateTime.UtcNow.AddDays(-3),
                    Vehicle = vehicle,
                    WorkOrder = workOrder,
                    Value = (decimal)estimate.ValueWithDiscount,
                });


                await _context.SaveChangesAsync();
            }
        }

        private async Task AddWorkOrderAsync()
        {
            if (!_context.WorkOrders.Any())
            {

                var appointment = await _context.Appointments.FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("eduardo_projeto_mecanicos@sapo.pt");
                var mechanic = await _context.Mechanics.Where(m => m.Email == "eduardo.sousa.moreno@formandos.cinel.pt").FirstOrDefaultAsync();

                _context.WorkOrders.Add(new WorkOrder
                {
                    Appointment = appointment,
                    awaitsReceipt = true,
                    CreatedBy = receptionist,
                    IsFinished = false,
                    OrderDateStart = DateTime.UtcNow.AddDays(-3).AddHours(-2),
                    OrderDateEnd = DateTime.UtcNow.AddDays(-3),
                    Status = "Opened",
                    UpdatedBy = receptionist,
                });

                appointment.AsAttended = true;
                _context.Appointments.Update(appointment);

                await _context.SaveChangesAsync();
            }

        }

        private async Task AddAppointmentAsync()
        {
            if (!_context.Appointments.Any())
            {
                var client = await _context.Clients.Where(c => c.Email == "eduardo.sousa.moreno@sapo.pt").FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "AA-TO-01").FirstOrDefaultAsync();
                var estimate = await _context.Estimates.FirstOrDefaultAsync();
                var mechanic = await _context.Mechanics.Where(m => m.Email == "eduardo.sousa.moreno@formandos.cinel.pt").FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("eduardo_projeto_mecanicos@sapo.pt");

                _context.Appointments.Add(new Appointment
                {
                    AppointmentStartDate = DateTime.UtcNow.AddDays(-3).AddHours(-2),
                    AppointmentEndDate = DateTime.UtcNow.AddDays(-3),
                    AppointmentServicesDetails = estimate.FaultDescription,
                    AsAttended = false,
                    CreatedBy = receptionist,
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    Client = client,
                    Estimate = estimate,
                    Mechanic = mechanic,
                    Vehicle = vehicle,
                    UpdatedBy = receptionist,
                    UpdatedDate = DateTime.UtcNow.AddDays(-5),
                    Observations = "Client doesn't want to align the direction",
                });

                estimate.HasAppointment = true;
                _context.Estimates.Update(estimate);

                await _context.SaveChangesAsync();
            }

        }

        private async Task AddClientVehiclesAsync()
        {
            if (!_context.Vehicles.Any())
            {

                var brand = await _context.Brands.Where(b => b.Name == "Mazda").FirstOrDefaultAsync();
                var model = await _context.Models.Where(m => m.Name == "MX-5 Miata").FirstOrDefaultAsync();
                var client = await _context.Clients.Where(c => c.Email == "eduardo.sousa.moreno@sapo.pt").FirstOrDefaultAsync();

                _context.Vehicles.Add(new Vehicle
                {
                    Brand = brand,
                    Model = model,
                    Client = client,
                    DateOfConstruction = DateTime.Now.AddYears(-5),
                    PlateNumber = "AA-TO-01",
                    Horsepower = 90,
                    ClientId = client.Id,
                });


                await _context.SaveChangesAsync();

            }
        }

        private async Task AddEstimateAsync()
        {
            if (!_context.Estimates.Any())
            {

                var client = await _context.Clients.Where(c => c.Email == "eduardo.sousa.moreno@sapo.pt").FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "AA-TO-01").FirstOrDefaultAsync();

                var estimate = new Estimate
                {
                    CreatedBy = await _userHelper.GetUserByEmailAsync("eduardo.sousa.moreno@formandos.cinel.pt"),
                    Client = client,
                    EstimateDate = DateTime.Now.AddDays(-5),
                    FaultDescription = "Blowed up Tyre, needs to replace front tyres.",
                    HasAppointment = false,
                    Vehicle = vehicle,
                };

                _context.Estimates.Add(estimate);

                await _context.SaveChangesAsync();

                var service = await _context.Services.Where(s => s.Name == "Change Tyres").FirstOrDefaultAsync();
                var estimateResult = await _context.Estimates.FirstOrDefaultAsync();

                var estimateDetail1 = new EstimateDetail
                {
                    ClientId = client.Id,
                    EstimateId = estimateResult.Id,
                    Service = service,
                    Quantity = 2,
                    Price = service.Price * 2,
                    VehicleId = vehicle.Id,
                };

                service = await _context.Services.Where(s => s.Name == "Michelin Tyres - Pilot Sport 5 - 221/30R19").FirstOrDefaultAsync();

                var estimateDetail2 = new EstimateDetail
                {
                    ClientId = client.Id,
                    EstimateId = estimateResult.Id,
                    Service = service,
                    Quantity = 2,
                    Price = service.PriceWithDiscount * 2,
                    VehicleId = vehicle.Id,
                };

                _context.EstimateDetails.Add(estimateDetail1);
                _context.EstimateDetails.Add(estimateDetail2);

                await _context.SaveChangesAsync();

            }

        }

        private async Task AddClientAsync()
        {
            if (!_context.Clients.Any())
            {
                // Client 1
                var clientUser1 = new User
                {
                    FirstName = "Eduardo",
                    LastName = "Moreno",
                    Email = "eduardo.sousa.moreno@sapo.pt",
                    UserName = "eduardo.sousa.moreno@sapo.pt",
                    PhoneNumber = "937919871",
                    Address = "Margem Sul"
                };

                _context.Clients.Add(new Client
                {
                    FirstName = "Eduardo",
                    LastName = "Moreno",
                    User = clientUser1,
                    Email = clientUser1.Email,
                    Nif = "235241660",
                    Address = clientUser1.Address,
                    PhoneNumber = clientUser1.PhoneNumber
                });

                await _userHelper.AddUserAsync(clientUser1, "123456");
                await _userHelper.AddUserToRoleAsync(clientUser1, "Client");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(clientUser1);
                await _userHelper.ConfirmEmailAsync(clientUser1, token);

                // Client 2

                var clientUser2 = new User
                {
                    FirstName = "Henrique",
                    LastName = "Martins",
                    Email = "henrique_martins99@sapo.pt", // ainda tenho q criar este email
                    UserName = "henrique_martins99@sapo.pt",
                    PhoneNumber = "935641236",
                    Address = "Areeiro"
                };

                _context.Clients.Add(new Client
                {
                    FirstName = "Henrique",
                    LastName = "Martins",
                    User = clientUser2,
                    Email = clientUser2.Email,
                    Nif = "256243800",
                    Address = clientUser2.Address,
                    PhoneNumber = clientUser2.PhoneNumber
                });

                await _userHelper.AddUserAsync(clientUser2, "123456");
                await _userHelper.AddUserToRoleAsync(clientUser2, "Client");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(clientUser2);
                await _userHelper.ConfirmEmailAsync(clientUser2, token);

                await _context.SaveChangesAsync();
            }

        }

        private async Task AddServicesAsync()
        {
            if (!_context.Services.Any())
            {
                _context.Services.Add(new Service
                {
                    Name = "Change Tyres",
                    Description = "Using the latest technology and tools to work your tyres. We change the valve...",
                    Price = 7.40m,
                    Discount = 0m
                });

                _context.Services.Add(new Service
                {
                    Name = "Michelin Tyres - Pilot Sport 5 - 221/30R19",
                    Description = "Best Michelin winter road tyres",
                    Price = 180.66m,
                    Discount = 10m
                });

                _context.Services.Add(new Service
                {
                    Name = "Oil Change",
                    Description = "We use the oil you choose and the way you want!",
                    Price = 29.90m,
                    Discount = 0m
                });

                _context.Services.Add(new Service
                {
                    Name = "Change Engine",
                    Description = "We change the engine for best performance.",
                    Price = 149.90m,
                    Discount = 10m
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddMechanicsRolesAsync()
        {
            if (!_context.MechanicsRoles.Any())
            {
                var specialties = new List<Speciality>();

                specialties.Add(new Speciality { Name = "Mechanic" });
                specialties.Add(new Speciality { Name = "Electrician" });
                specialties.Add(new Speciality { Name = "Painter" });

                _context.MechanicsRoles.Add(new Role
                {
                    Specialities = specialties,
                    Name = "Technician",
                    PermissionsName = "Technician"
                });

                var receptionistSpecialties = new List<Speciality>();

                receptionistSpecialties.Add(new Speciality { Name = "Generalist" });
                _context.MechanicsRoles.Add(new Role
                {
                    Specialities = receptionistSpecialties,
                    Name = "Receptionist",
                    PermissionsName = "Receptionist"
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddBrandsAsync()
        {
            if (!_context.Brands.Any())
            {
                var models = new List<Model>();

                models.Add(new Model { Name = "MX-5 Miata" });
                models.Add(new Model { Name = "CX-90" });
                models.Add(new Model { Name = "CX-30" });
                models.Add(new Model { Name = "6" });
                models.Add(new Model { Name = "RX-7" });
                models.Add(new Model { Name = "RX-8" });

                _context.Brands.Add(new Brand
                {
                    Models = models,
                    Name = "Mazda"
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCreatedRoles()
        {
            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Technician");
            await _userHelper.CheckRoleAsync("Receptionist");
            await _userHelper.CheckRoleAsync("Client");
        }

        private async Task AddUserAsync()
        {
            var user = await _userHelper.GetUserByEmailAsync("f92ferreira@gmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Filipe",
                    LastName = "Ferreira",
                    Email = "f92ferreira@gmail.com",
                    UserName = "f92ferreira@gmail.com",
                    PhoneNumber = "925648979",
                    Address = "Avenida Fialho Gouveia"
                };

                await _userHelper.AddUserAsync(user, "123456");
            }

            var isInRole = await _userHelper.CheckUserInRoleAsync(user, "Admin");

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            await _userHelper.ConfirmEmailAsync(user, token);

            await _context.SaveChangesAsync();
        }

        private async Task AddMechanicsAsync()
        {
            if (!_context.Mechanics.Any())
            {
                // Mechanic 1
                var mechanicUser1 = new User
                {
                    FirstName = "Joaquim",
                    LastName = "Guedes",
                    Email = "joaquimguedes@yopmail.com",
                    UserName = "joaquimguedes@yopmail.com",
                    PhoneNumber = "965897956",
                    Address = "Rua do barco"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Joaquim",
                    LastName = "Guedes",
                    About = "Born in Lisbon, Joaquim Guedes started his electrician carrer in Bosch Car Service in Lisbon...",
                    User = mechanicUser1,
                    Role = _context.MechanicsRoles.Where(r => r.Name == "Technician").FirstOrDefault(),
                    Speciality = _context.Specialities.Where(s => s.Name == "Electrician").FirstOrDefault(),
                    Email = mechanicUser1.Email,
                    Color = "#8F6593"
                });

                await _userHelper.AddUserAsync(mechanicUser1, "123456");
                await _userHelper.AddUserToRoleAsync(mechanicUser1, "Technician");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(mechanicUser1);
                await _userHelper.ConfirmEmailAsync(mechanicUser1, token);

                // Mechanic 2
                var mechanicUser2 = new User
                {
                    FirstName = "Inacio",
                    LastName = "Torres",
                    Email = "inaciotorres@yopmail.com",
                    UserName = "inaciotorres@yopmail.com",
                    PhoneNumber = "965896425",
                    Address = "Rua das Bananas"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Inacio",
                    LastName = "Torres",
                    About = "Born in Setúbal, Inacio Torres studied mechatronics in ATEC and then joined Garage88, with 4 years of experience..",
                    User = mechanicUser2,
                    Role = _context.MechanicsRoles.Where(r => r.Name == "Technician").FirstOrDefault(),
                    Speciality = _context.Specialities.Where(s => s.Name == "Mechanic").FirstOrDefault(),
                    Email = mechanicUser2.Email,
                    Color = "#3066BE"
                });

                await _userHelper.AddUserAsync(mechanicUser2, "123456");
                await _userHelper.AddUserToRoleAsync(mechanicUser2, "Technician");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(mechanicUser2);
                await _userHelper.ConfirmEmailAsync(mechanicUser2, token);

                // Mechanic 3

                var mechanicUser3 = new User
                {
                    FirstName = "Pedro",
                    LastName = "Rato",
                    Email = "Pedrorato@yopmail.com",
                    UserName = "Pedrorato@yopmail.com",
                    PhoneNumber = "923548965",
                    Address = "Rua do Alicate"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Pedro",
                    LastName = "Rato",
                    About = "There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don't look even slightly believable. If you are going to use a passage of Lorem Ipsum, you need to be sure there isn't anything embarrassing hidden in the middle of text. ",
                    User = mechanicUser3,
                    Role = _context.MechanicsRoles.Where(r => r.Name == "Receptionist").FirstOrDefault(),
                    Speciality = _context.Specialities.Where(s => s.Name == "Generalist").FirstOrDefault(),
                    Email = mechanicUser3.Email
                });

                await _userHelper.AddUserAsync(mechanicUser3, "123456");
                await _userHelper.AddUserToRoleAsync(mechanicUser3, "Receptionist");
                token = await _userHelper.GenerateEmailConfirmationTokenAsync(mechanicUser3);
                await _userHelper.ConfirmEmailAsync(mechanicUser3, token);

                await _context.SaveChangesAsync();

            }
        }
    }
}

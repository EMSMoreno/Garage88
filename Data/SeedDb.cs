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

            // Add specialties
            var specialities = new List<Speciality>
            {
                new Speciality { Name = "Change Tires" },
                new Speciality { Name = "Engine Repair" },
                new Speciality { Name = "General Mechanics" },
                new Speciality { Name = "Automotive Electronics" },
                new Speciality { Name = "Wheel Alignment and Balancing" },
                new Speciality { Name = "Suspension and Steering" },
                new Speciality { Name = "Oil Change" },
                new Speciality { Name = "Automotive Painting" },
                new Speciality { Name = "Bodywork Services" },
                new Speciality { Name = "Accessory Installation" },
                new Speciality { Name = "Cooling System Service" }
            };

            await _context.Specialities.AddRangeAsync(specialities);
            await _context.SaveChangesAsync();

            // Add roles
            var mechanicRole = new Role { Name = "Mechanic" };
            var seniorMechanicRole = new Role { Name = "Senior Mechanic" };
            await _context.MechanicsRoles.AddRangeAsync(mechanicRole, seniorMechanicRole);
            await _context.SaveChangesAsync();

            // Add mechanics
            var mechanics = new List<Mechanic>
            {
                new Mechanic { FirstName = "John", LastName = "Doe", Email = "john.doe@garage.pt", About = "Mechanic with 4 years of experience.", SpecialityId = specialities[0].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@garage.pt", About = "Mechanic with 5 years of experience.", SpecialityId = specialities[1].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Michael", LastName = "Brown", Email = "michael.brown@garage.pt", About = "Mechanic with 6 years of experience.", SpecialityId = specialities[2].Id, RoleId = seniorMechanicRole.Id },
                new Mechanic { FirstName = "Emily", LastName = "Johnson", Email = "emily.johnson@garage.pt", About = "Senior mechanic with 7 years of experience.", SpecialityId = specialities[3].Id, RoleId = seniorMechanicRole.Id },
                new Mechanic { FirstName = "David", LastName = "Miller", Email = "david.miller@garage.pt", About = "Mechanic with 4 years of experience.", SpecialityId = specialities[4].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Sophia", LastName = "Davis", Email = "sophia.davis@garage.pt", About = "Mechanic with 5 years of experience.", SpecialityId = specialities[5].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "James", LastName = "Wilson", Email = "james.wilson@garage.pt", About = "Mechanic with 6 years of experience.", SpecialityId = specialities[6].Id, RoleId = seniorMechanicRole.Id },
                new Mechanic { FirstName = "Olivia", LastName = "Moore", Email = "olivia.moore@garage.pt", About = "Senior mechanic with 7 years of experience.", SpecialityId = specialities[7].Id, RoleId = seniorMechanicRole.Id },
                new Mechanic { FirstName = "Daniel", LastName = "Taylor", Email = "daniel.taylor@garage.pt", About = "Mechanic with 4 years of experience.", SpecialityId = specialities[8].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Ava", LastName = "Anderson", Email = "ava.anderson@garage.pt", About = "Mechanic with 5 years of experience.", SpecialityId = specialities[9].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Benjamin", LastName = "Thomas", Email = "benjamin.thomas@garage.pt", About = "Mechanic with 6 years of experience.", SpecialityId = specialities[10].Id, RoleId = seniorMechanicRole.Id },
                new Mechanic { FirstName = "Isabella", LastName = "Jackson", Email = "isabella.jackson@garage.pt", About = "Senior mechanic with 7 years of experience.", SpecialityId = specialities[0].Id, RoleId = seniorMechanicRole.Id },
                new Mechanic { FirstName = "Lucas", LastName = "White", Email = "lucas.white@garage.pt", About = "Mechanic with 4 years of experience.", SpecialityId = specialities[1].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Mia", LastName = "Harris", Email = "mia.harris@garage.pt", About = "Mechanic with 5 years of experience.", SpecialityId = specialities[2].Id, RoleId = mechanicRole.Id },
                new Mechanic { FirstName = "Ethan", LastName = "Martin", Email = "ethan.martin@garage.pt", About = "Mechanic with 6 years of experience.", SpecialityId = specialities[3].Id, RoleId = seniorMechanicRole.Id }
            };

            await _context.Mechanics.AddRangeAsync(mechanics);
            await _context.SaveChangesAsync();

            // Add clients
            var client = new Client { FirstName = "Jéssica", LastName = "Guimarães", Nif = "222908765", Address = "Avenida D.Luis III", Email = "jessicaG@yopmail.com", PhoneNumber = "916969696" };

            var client2 = new Client { FirstName = "Prof.", LastName = "X", Nif = "000100100", Address = "Avenida D.Luis II", Email = "xmen@yopmail.com", PhoneNumber = "930006750" };

            var client3 = new Client { FirstName = "Diogo", LastName = "Ramos", Nif = "765656789", Address = "Avenida D.Luis I", Email = "diogor@yopmail.com", PhoneNumber = "213331212" };

            var client4 = new Client { FirstName = "Tiago", LastName = "Ribeiro", Nif = "666578432", Address = "D. Carlos I", Email = "tiago_ribeiro@yopmail.com", PhoneNumber = "989909873" };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // Add brands and models
            var brand = new Brand { Name = "Mazda" };
            var brand2 = new Brand { Name = "Toyota" };
            var brand3 = new Brand { Name = "BMW" };
            var brand4 = new Brand { Name = "Ford" };
            var brand5 = new Brand { Name = "Tesla" };
            var brand6 = new Brand { Name = "Mercedes-Benz" };
            _context.Brands.Add(brand);

            var model = new Model { Name = "Miata MX-5", BrandId = brand.Id };
            var model2 = new Model { Name = "Supra", BrandId = brand.Id };
            var model3 = new Model { Name = "M3", BrandId = brand.Id };
            var model4 = new Model { Name = "Mustang GT", BrandId = brand.Id };
            var model5 = new Model { Name = "Model S", BrandId = brand.Id };
            var model6 = new Model { Name = "AMG GT", BrandId = brand.Id };
            _context.Models.Add(model);
            await _context.SaveChangesAsync();

            // Add vehicles and make association w/models and clients
            var vehicle = new Vehicle { PlateNumber = "MX50555", ModelId = model.Id, ClientId = client.Id };
            var vehicle2 = new Vehicle { PlateNumber = "TP12345", ModelId = model.Id, ClientId = client.Id };
            var vehicle3 = new Vehicle { PlateNumber = "MB32321", ModelId = model.Id, ClientId = client.Id };
            var vehicle4 = new Vehicle { PlateNumber = "FD67890", ModelId = model.Id, ClientId = client.Id };
            var vehicle5 = new Vehicle { PlateNumber = "TSL0001", ModelId = model.Id, ClientId = client.Id };
            var vehicle6 = new Vehicle { PlateNumber = "MBZGT20", ModelId = model.Id, ClientId = client.Id };

            _context.Vehicles.Add(vehicle);

            // Add services
            var service = new Service { Name = "Oil Change", Price = 29.99m, Description = "Standard oil change including oil filter replacement.", Discount = 10.00m };
            var service2 = new Service { Name = "Brake Pad Replacement", Price = 150.00m, Description = "Replacement of front and rear brake pads.", Discount = 5.00m };
            var service3 = new Service { Name = "Full Vehicle Inspection", Price = 99.99m, Description = "Comprehensive vehicle inspection covering all critical systems.", Discount = 15.00m };
            var service4 = new Service { Name = "Tire Rotation and Balancing", Price = 49.95m, Description = "Tire rotation and balancing to ensure even tire wear and smooth driving.", Discount = 0.00m };
            var service5 = new Service { Name = "Air Conditioning Service", Price = 89.99m, Description = "Air conditioning check and refill to ensure proper cooling.", Discount = 8.50m };
            var service6 = new Service { Name = "Transmission Fluid Replacement", Price = 180.00m, Description = "Complete transmission fluid replacement service.", Discount = 12.00m };

            _context.Services.Add(service);

            // Add appointments and associations w/clients
            var appointment = new Appointment
            {
                Observations = "Initial consultation",
                ClientId = client.Id,  // association w/client
                VehicleId = vehicle.Id,  // association w/vehicle
                   
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
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "OPL65437").FirstOrDefaultAsync();
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
                    PlateNumber = "OPL65437",
                    Horsepower = 97,
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
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "OPL65437").FirstOrDefaultAsync();

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
                    Email = "henrique_martins99@sapo.pt", // still needs to create this
                    UserName = "henrique_martins99@sapo.pt",
                    PhoneNumber = "935641236",
                    Address = "Aveiro"
                };

                _context.Clients.Add(new Client
                {
                    FirstName = "Henrique",
                    LastName = "Sousa",
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
                    Description = "Using the latest technology and tools to work your tyres.",
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
            var user = await _userHelper.GetUserByEmailAsync("rafaferreira@gmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Rafael",
                    LastName = "Ferreira",
                    Email = "rafaferreira@gmail.com",
                    UserName = "rafaferreira@gmail.com",
                    PhoneNumber = "925648980",
                    Address = "Avenida da Liberdade"
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
                    FirstName = "David",
                    LastName = "Pacheco",
                    Email = "davidpacheco@yopmail.com",
                    UserName = "davidpacheco@yopmail.com",
                    PhoneNumber = "965897956",
                    Address = "Rua Luisa Todi"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "David",
                    LastName = "Pacheco",
                    About = "Born in Bragança, David Pacheco started his electrician career in Bosch Car Service in Lisbon...",
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
                    FirstName = "Beatriz",
                    LastName = "Godinho",
                    Email = "beagodinho@yopmail.com",
                    UserName = "beagodinho@yopmail.com",
                    PhoneNumber = "965896421",
                    Address = "Rua das Bananas"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Beatriz",
                    LastName = "Godinho",
                    About = "Born in Setúbal, Beatriz Godinho studied cybersecurity at CINEL and then joined Garage88, with 4 years of experience.",
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
                    LastName = "Dinis",
                    Email = "Pedrodinis@yopmail.com",
                    UserName = "Pedrodinis@yopmail.com",
                    PhoneNumber = "923548445",
                    Address = "Rua das Batatas Fritas"
                };

                _context.Mechanics.Add(new Mechanic
                {
                    FirstName = "Pedro",
                    LastName = "Dinis",
                    About = "Pedro is just a guy that keeps breaking his vehicle glasses, so he is one of our best clients. ",
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

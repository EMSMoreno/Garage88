using Microsoft.EntityFrameworkCore;
using Garage88.Helpers;
using Garage88.Data;
using Garage88.Data.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Syncfusion.EJ2.FileManager;

namespace Garage88.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedDb(DataContext context, IUserHelper userHelper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userHelper = userHelper;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await CheckCreatedRoles();
            await AddUserAsync();
            await AddIdentityRolesAsync();
            await AddMechanicsRolesAsync();
            await AddMechanicsAsync();
            await AddBrandsAsync();
            await AddServicesAsync();
            await AddClientAsync();
            await AddClientVehiclesAsync();
            await AddEstimateAsync();
            await AddAppointmentAsync();
            await AddWorkOrderAsync();
            await AddInvoiceAsync();
        }

        private async Task AddInvoiceAsync()
        {
            if (!_context.Invoices.Any())
            {
                var workOrder = await _context.WorkOrders.FirstOrDefaultAsync();

                if (workOrder == null)
                {
                    Console.WriteLine("No work order found.");
                    return;
                }

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

                if (appointment == null)
                {
                    Console.WriteLine("No appointment found.");
                    return;
                }

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
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "MZD68439").FirstOrDefaultAsync();
                var estimate = await _context.Estimates.FirstOrDefaultAsync();
                var mechanic = await _context.Mechanics.Where(m => m.Email == "eduardo.sousa.moreno@formandos.cinel.pt").FirstOrDefaultAsync();
                var receptionist = await _userHelper.GetUserByEmailAsync("eduardo_projeto_mecanicos@sapo.pt");

                // Verifique se algum dos objetos é null
                if (client == null || vehicle == null || estimate == null || mechanic == null || receptionist == null)
                {
                    Console.WriteLine("One or more required entities are null.");
                    return;  // Ou lançar uma exceção, conforme o comportamento desejado
                }

                // Adicionando o Appointment
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

                // Atualizando a estimativa
                estimate.HasAppointment = true;
                _context.Estimates.Update(estimate);

                await _context.SaveChangesAsync();

                if (client == null)
                {
                    throw new Exception("Client not found.");
                }

                if (vehicle == null)
                {
                    throw new Exception("Vehicle not found.");
                }

                if (estimate == null)
                {
                    throw new Exception("Estimate not found.");
                }

                if (mechanic == null)
                {
                    throw new Exception("Mechanic not found.");
                }

                if (receptionist == null)
                {
                    throw new Exception("Receptionist not found.");
                }
            }

        }

        private async Task AddClientVehiclesAsync()
        {
            if (!_context.Vehicles.Any())
            {
                var brand = await _context.Brands.Where(b => b.Name == "Mazda").FirstOrDefaultAsync();
                var model = await _context.Models.Where(m => m.Name == "MX-5 Miata").FirstOrDefaultAsync();
                var client = await _context.Clients.Where(c => c.Email == "eduardo.sousa.moreno@sapo.pt").FirstOrDefaultAsync();

                // Check for null variables before proceeding
                if (brand == null)
                {
                    Console.WriteLine("Brand 'Mazda' not found.");
                    return;
                }

                if (model == null)
                {
                    Console.WriteLine("Model 'MX-5 Miata' not found.");
                    return;
                }

                if (client == null)
                {
                    Console.WriteLine("Client with email 'eduardo.sousa.moreno@sapo.pt' not found.");
                    return;
                }

                _context.Vehicles.Add(new Vehicle
                {
                    Brand = brand,
                    Model = model,
                    Client = client,
                    DateOfConstruction = DateTime.Now.AddYears(-5),
                    PlateNumber = "MZD68439",
                    Horsepower = 97,
                    ClientId = client.Id,
                    VehicleIdentificationNumber = "MZD82633A123456"
                });

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddEstimateAsync()
        {
            if (!_context.Estimates.Any())
            {
                var client = await _context.Clients.Where(c => c.Email == "eduardo.sousa.moreno@sapo.pt").FirstOrDefaultAsync();
                var vehicle = await _context.Vehicles.Where(v => v.PlateNumber == "MZD68439").FirstOrDefaultAsync();

                var createdByUser = await _userHelper.GetUserByEmailAsync("eduardo.sousa.moreno@formandos.cinel.pt");

                if (createdByUser == null)
                {
                    Console.WriteLine("User not found.");
                    return; // Retorna caso o usuário não seja encontrado
                }

                var estimate = new Estimate
                {
                    CreatedBy = createdByUser,
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

        private async Task AddIdentityRolesAsync()
        {
            var roles = new List<string> { "ADMIN", "CLIENT", "MECHANIC" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role, NormalizedName = role.ToUpper() });
                }
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
            var user = await _userHelper.GetUserByEmailAsync("rafaelsantos@yopmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Rafael",
                    LastName = "Santos",
                    Email = "rafaelsantos@yopmail.com",
                    UserName = "rafaelsantos@yopmail.com",
                    PhoneNumber = "925648980",
                    Address = "Avenida da Liberdade"
                };

                await _userHelper.AddUserAsync(user, "RafaelSantos!4");
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

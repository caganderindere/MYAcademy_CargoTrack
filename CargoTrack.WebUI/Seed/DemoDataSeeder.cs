using CargoTrack.Business.Rules;
using CargoTrack.Business.Services.CargoPricing;
using CargoTrack.DataAccess.Context;
using CargoTrack.DTO.DTOs.Cargos;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.WebUI.Seed
{
    public static class DemoDataSeeder
    {
        private const int CargoCount = 48;
        private const int HistoryDays = 12;

        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            if (!configuration.GetValue<bool>("DemoData:Enabled"))
            {
                return;
            }

            var context = services.GetRequiredService<AppDbContext>();

            if (await context.Branches.AnyAsync())
            {
                return;
            }

            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var pricingService = services.GetRequiredService<ICargoPricingService>();
            var password = configuration["DemoData:Password"]!;
            var random = new Random(2026);
            var cities = await context.Cities.ToDictionaryAsync(x => x.Name);

            var centers = new[]
            {
                new TransferCenter { Name = "Marmara Aktarma Merkezi", CityId = cities["Kocaeli"].Id, Address = "Gebze OSB 3. Cadde No:4" },
                new TransferCenter { Name = "İç Anadolu Aktarma Merkezi", CityId = cities["Ankara"].Id, Address = "Sincan Lojistik Vadisi No:18" },
                new TransferCenter { Name = "Ege ve Akdeniz Aktarma Merkezi", CityId = cities["İzmir"].Id, Address = "Kemalpaşa OSB 12. Sokak No:7" }
            };
            context.TransferCenters.AddRange(centers);

            var branchData = new (string Name, string City, int Center, string Phone, string Address)[]
            {
                ("Kadıköy Şubesi", "İstanbul", 0, "02163450011", "Caferağa Mah. Moda Cad. No:42 Kadıköy"),
                ("Beşiktaş Şubesi", "İstanbul", 0, "02122670022", "Sinanpaşa Mah. Ihlamurdere Cad. No:15 Beşiktaş"),
                ("Nilüfer Şubesi", "Bursa", 0, "02244410033", "Fethiye Mah. FSM Bulvarı No:120 Nilüfer"),
                ("İzmit Şubesi", "Kocaeli", 0, "02623210044", "Yenişehir Mah. Ankara Yolu No:8 İzmit"),
                ("Çankaya Şubesi", "Ankara", 1, "03124670055", "Kızılay Mah. Atatürk Bulvarı No:88 Çankaya"),
                ("Melikgazi Şubesi", "Kayseri", 1, "03522310066", "Sahabiye Mah. Sivas Cad. No:21 Melikgazi"),
                ("Konak Şubesi", "İzmir", 2, "02324450077", "Alsancak Mah. Kıbrıs Şehitleri Cad. No:63 Konak"),
                ("Muratpaşa Şubesi", "Antalya", 2, "02423220088", "Şirinyalı Mah. Lara Cad. No:204 Muratpaşa")
            };

            var branches = branchData.Select(x => new Branch
            {
                Name = x.Name,
                CityId = cities[x.City].Id,
                TransferCenter = centers[x.Center],
                PhoneNumber = x.Phone,
                Address = x.Address
            }).ToList();
            context.Branches.AddRange(branches);

            var firstNames = new[] { "Ahmet", "Ayşe", "Mehmet", "Elif", "Mustafa", "Zeynep", "Emre", "Selin", "Burak", "Deniz", "Can", "Ece", "Kerem", "Merve", "Onur", "Gizem" };
            var lastNames = new[] { "Yılmaz", "Kaya", "Demir", "Şahin", "Çelik", "Yıldız", "Aydın", "Öztürk", "Arslan", "Doğan", "Koç", "Kurt" };

            var employees = new List<Employee>();
            for (var i = 0; i < branches.Count; i++)
            {
                employees.Add(new Employee { FirstName = firstNames[i], LastName = lastNames[i % lastNames.Length], PhoneNumber = $"05321000{i:D3}", Position = EmployeePosition.BranchManager, Branch = branches[i] });
                employees.Add(new Employee { FirstName = firstNames[(i + 5) % firstNames.Length], LastName = lastNames[(i + 3) % lastNames.Length], PhoneNumber = $"05322000{i:D3}", Position = EmployeePosition.Courier, Branch = branches[i] });
                employees.Add(new Employee { FirstName = firstNames[(i + 9) % firstNames.Length], LastName = lastNames[(i + 7) % lastNames.Length], PhoneNumber = $"05323000{i:D3}", Position = EmployeePosition.BranchStaff, Branch = branches[i] });
            }
            for (var i = 0; i < centers.Length; i++)
            {
                employees.Add(new Employee { FirstName = firstNames[(i + 2) % firstNames.Length], LastName = lastNames[(i + 5) % lastNames.Length], PhoneNumber = $"05324000{i:D3}", Position = EmployeePosition.TransferCenterStaff, TransferCenter = centers[i] });
            }
            context.Employees.AddRange(employees);

            var customers = new List<Customer>();
            var addresses = new List<Address>();
            for (var i = 0; i < 14; i++)
            {
                var branch = branches[i % branches.Count];
                var customer = new Customer
                {
                    FirstName = firstNames[(i + 3) % firstNames.Length],
                    LastName = lastNames[(i + 1) % lastNames.Length],
                    PhoneNumber = $"0555{(1000000 + i * 7919):D7}",
                    Email = $"musteri{i + 1}@ornek.com"
                };
                customers.Add(customer);
                addresses.Add(new Address { Customer = customer, Title = i % 2 == 0 ? "Ev" : "İş", CityId = branch.CityId, District = branch.Name.Replace(" Şubesi", ""), FullAddress = $"Cumhuriyet Mah. {i + 3}. Sokak No:{i + 10} Daire:{i % 9 + 1}" });
            }
            context.Customers.AddRange(customers);
            context.Addresses.AddRange(addresses);

            await context.SaveChangesAsync();

            await CreateAccountAsync(userManager, "kadikoy.mudur", employees[0].FirstName, employees[0].LastName, password, Roles.Manager, user => employees[0].AppUserId = user.Id);
            await CreateAccountAsync(userManager, "cankaya.mudur", employees[12].FirstName, employees[12].LastName, password, Roles.Manager, user => employees[12].AppUserId = user.Id);
            await CreateAccountAsync(userManager, "demo.musteri", customers[0].FirstName, customers[0].LastName, password, Roles.User, user => customers[0].AppUserId = user.Id);
            await context.SaveChangesAsync();

            var targets = new[]
            {
                CargoStatus.Delivered, CargoStatus.Delivered, CargoStatus.Delivered, CargoStatus.Delivered, CargoStatus.Delivered,
                CargoStatus.OutForDelivery, CargoStatus.AtDestinationBranch, CargoStatus.InTransferCenter, CargoStatus.AtOriginBranch,
                CargoStatus.Created, CargoStatus.DeliveryFailed, CargoStatus.Delivered, CargoStatus.ReturnInProgress, CargoStatus.ReturnedToSender
            };

            var createdDates = new Dictionary<Guid, DateTime>();

            for (var i = 0; i < CargoCount; i++)
            {
                var senderIndex = i == 0 || i % 7 == 0 ? 0 : random.Next(customers.Count);
                var receiverIndex = (senderIndex + 1 + random.Next(customers.Count - 1)) % customers.Count;
                if (i % 5 == 1)
                {
                    receiverIndex = 0;
                    senderIndex = senderIndex == 0 ? 1 : senderIndex;
                }

                var origin = i % 3 == 0 ? branches[0] : branches[random.Next(branches.Count)];
                var receiver = customers[receiverIndex];
                var address = addresses.First(x => x.Customer == receiver);
                var destination = branches.First(x => x.CityId == address.CityId);
                var target = targets[i % targets.Length];
                var daysAgo = target is CargoStatus.Created or CargoStatus.AtOriginBranch or CargoStatus.OutForDelivery ? random.Next(0, 2) : random.Next(0, HistoryDays);
                var created = DateTime.Today.AddDays(-daysAgo).AddHours(8 + random.Next(0, 9)).AddMinutes(random.Next(0, 60));
                var deliveryType = origin.CityId == destination.CityId && i % 4 == 0 ? DeliveryType.SameDay : i % 3 == 0 ? DeliveryType.Express : DeliveryType.Standard;
                var additionalServices = (AdditionalService)(i % 4 == 0 ? 1 : i % 5 == 0 ? 6 : 0);

                var dto = new CreateCargoDto
                {
                    SenderId = customers[senderIndex].Id,
                    ReceiverId = receiver.Id,
                    DeliveryAddressId = address.Id,
                    OriginBranchId = origin.Id,
                    DestinationBranchId = destination.Id,
                    Weight = Math.Round((decimal)(0.5 + random.NextDouble() * 14), 1),
                    Width = random.Next(10, 60),
                    Length = random.Next(10, 60),
                    Height = random.Next(5, 40),
                    CargoType = (CargoType)(i % 4 + 1),
                    DeliveryType = deliveryType,
                    AdditionalServices = additionalServices
                };

                var (desi, price) = await pricingService.CalculateAsync(dto);

                var cargo = new Cargo
                {
                    TrackCode = $"MYC-{created.Year}-{random.Next(100000, 1000000)}",
                    SenderId = dto.SenderId,
                    ReceiverId = dto.ReceiverId,
                    DeliveryAddressId = dto.DeliveryAddressId,
                    OriginBranchId = dto.OriginBranchId,
                    DestinationBranchId = dto.DestinationBranchId,
                    Weight = dto.Weight,
                    Width = dto.Width,
                    Length = dto.Length,
                    Height = dto.Height,
                    Desi = desi,
                    Price = price,
                    CargoType = dto.CargoType,
                    DeliveryType = dto.DeliveryType,
                    AdditionalServices = dto.AdditionalServices,
                    EstimatedDeliveryDate = DeliveryDateRules.CalculateEstimatedDeliveryDate(created, dto.DeliveryType, dto.AdditionalServices),
                    Movements = new List<CargoMovement>(),
                    DeliveryExceptions = new List<DeliveryException>()
                };

                BuildJourney(cargo, origin, destination, centers, employees, target, created, random, $"{receiver.FirstName} {receiver.LastName}");

                context.Cargos.Add(cargo);
                createdDates[cargo.Id] = created;
            }

            await context.SaveChangesAsync();

            foreach (var (id, created) in createdDates)
            {
                await context.Cargos.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.CreatedDate, created));
            }
        }

        private static void BuildJourney(Cargo cargo, Branch origin, Branch destination, TransferCenter[] centers, List<Employee> employees, CargoStatus target, DateTime start, Random random, string receiverName)
        {
            var time = start;
            var status = CargoStatus.Created;
            var originStaff = employees.First(x => x.Branch == origin && x.Position == EmployeePosition.BranchStaff);
            var destinationStaff = employees.First(x => x.Branch == destination && x.Position == EmployeePosition.BranchStaff);
            var courier = employees.First(x => x.Branch == destination && x.Position == EmployeePosition.Courier);
            var center = origin.TransferCenter ?? centers[0];
            var centerStaff = employees.First(x => x.TransferCenter == center);

            void Move(CargoStatus to, Branch? branch, TransferCenter? transferCenter, Employee? employee, string description, double hours)
            {
                time = time.AddHours(hours).AddMinutes(random.Next(0, 50));
                if (time > DateTime.Now)
                {
                    time = DateTime.Now.AddMinutes(-random.Next(5, 90));
                }

                cargo.Movements.Add(new CargoMovement
                {
                    FromStatus = to == CargoStatus.Created ? null : status,
                    ToStatus = to,
                    MovementDate = time,
                    Branch = branch,
                    TransferCenter = transferCenter,
                    Employee = employee,
                    Description = description
                });
                status = to;
            }

            Move(CargoStatus.Created, origin, null, originStaff, "Gönderi oluşturuldu.", 0);
            if (target == CargoStatus.Created) { cargo.CargoStatus = status; return; }

            Move(CargoStatus.AtOriginBranch, origin, null, originStaff, "Gönderi şubede kabul edildi, ölçüm ve etiketleme yapıldı.", 0.5);
            if (target == CargoStatus.AtOriginBranch) { cargo.CargoStatus = status; return; }

            if (origin.CityId != destination.CityId)
            {
                Move(CargoStatus.InTransferCenter, null, center, centerStaff, "Aktarma merkezine ulaştı.", 6);
                if (target == CargoStatus.InTransferCenter) { cargo.CargoStatus = status; return; }
            }

            Move(CargoStatus.AtDestinationBranch, destination, null, destinationStaff, "Varış şubesine ulaştı.", 10);
            if (target == CargoStatus.AtDestinationBranch || target == CargoStatus.InTransferCenter) { cargo.CargoStatus = status; return; }

            var attempts = target switch
            {
                CargoStatus.DeliveryFailed => 1,
                CargoStatus.ReturnInProgress or CargoStatus.ReturnedToSender => 3,
                _ => random.Next(0, 4) == 0 ? 1 : 0
            };

            var reasons = new[] { DeliveryExceptionReason.RecipientNotAvailable, DeliveryExceptionReason.WrongAddress, DeliveryExceptionReason.Refused };

            for (var attempt = 1; attempt <= attempts; attempt++)
            {
                Move(CargoStatus.OutForDelivery, destination, null, courier, "Kurye dağıtıma çıktı.", attempt == 1 ? 3 : 18);
                Move(CargoStatus.DeliveryFailed, destination, null, courier, $"Teslim edilemedi ({attempt}. deneme).", 2);
                cargo.FailedDeliveryAttempts = attempt;
                cargo.DeliveryExceptions.Add(new DeliveryException
                {
                    Employee = courier,
                    Reason = reasons[(attempt - 1) % reasons.Length],
                    AttemptNumber = attempt,
                    OccurredDate = time,
                    Description = attempt == 1 ? "Alıcı adreste bulunamadı, bilgi notu bırakıldı." : null
                });
            }

            if (target == CargoStatus.DeliveryFailed) { cargo.CargoStatus = status; return; }

            if (attempts >= 3)
            {
                Move(CargoStatus.ReturnInProgress, destination, null, null, "3 başarısız teslimat denemesi sonrası iade süreci başlatıldı.", 0.1);
                if (target == CargoStatus.ReturnedToSender)
                {
                    Move(CargoStatus.ReturnedToSender, origin, null, originStaff, "Gönderi göndericiye iade edildi.", 30);
                }
                cargo.CargoStatus = status;
                return;
            }

            Move(CargoStatus.OutForDelivery, destination, null, courier, "Kurye dağıtıma çıktı.", attempts == 0 ? 3 : 18);

            if (target == CargoStatus.OutForDelivery)
            {
                cargo.CargoStatus = status;
                cargo.DeliveryCode = random.Next(0, 1000000).ToString("D6");
                cargo.DeliveryCodeGeneratedDate = time;
                return;
            }

            Move(CargoStatus.Delivered, destination, null, courier, "Teslim edildi.", 2.5 + random.NextDouble() * (random.Next(0, 5) == 0 ? 60 : 4));
            cargo.CargoStatus = CargoStatus.Delivered;
            cargo.DeliveredDate = time;
            cargo.Delivery = new Delivery { Employee = courier, ReceivedByName = receiverName, DeliveredDate = time, Note = "Kod doğrulanarak teslim edildi." };
        }

        private static async Task CreateAccountAsync(UserManager<AppUser> userManager, string userName, string firstName, string lastName, string password, string role, Action<AppUser> link)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user is null)
            {
                user = new AppUser { UserName = userName, Email = $"{userName}@cargotrack.com.tr", FirstName = firstName, LastName = lastName, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Demo kullanıcısı oluşturulamadı: " + string.Join(", ", result.Errors.Select(x => x.Description)));
                }

                await userManager.AddToRoleAsync(user, role);
            }

            link(user);
        }
    }
}

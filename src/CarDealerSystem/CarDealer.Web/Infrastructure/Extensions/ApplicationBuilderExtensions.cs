namespace CarDealer.Web.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Transactions;
    using CarDealer.Models;
    using CarDealer.Models.BasicTypes;
    using CarDealer.Models.Enums;
    using Common;
    using Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Middleware;
    using Models.Dtos;
    using Newtonsoft.Json;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder SeedData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<CarDealerDbContext>();

                dbContext.Database.Migrate();
                SeedRequiredData(dbContext);

                // Delete (Not recommended) or comment this method if you'd like to NOT have initial database with users and vehicles.
                SeedOptionalData(serviceScope, dbContext);
            }

            return app;
        }

        public static IApplicationBuilder AddDefaultSecurityHeaders(this IApplicationBuilder app, SecurityHeadersBuilder builder)
            => app.UseMiddleware<SecurityHeadersMiddleware>(builder.Policy());

        #region SeedDataRegion

        private static void SeedRequiredData(CarDealerDbContext dbContext)
        {
            SeedManufacturersWithModels(dbContext);
            SeedFuelTypes(dbContext);
            SeedTransmissionTypes(dbContext);
            SeedFeatures(dbContext);
        }

        private static void SeedOptionalData(IServiceScope serviceScope, CarDealerDbContext dbContext)
        {
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            SeedDefaultRoles(userManager, roleManager);
            SeedUsers(userManager, dbContext);
            SeedAds(dbContext);
        }

        private static void SeedAds(CarDealerDbContext dbContext)
        {
            if (!dbContext.Vehicles.Any())
            {
                var random = new Random();
                var usersIds = dbContext.Users.Select(u => u.Id).ToList();
                var manufacturers = dbContext.Manufacturers.Include(m => m.Models).ToList();
                var fuelTypesIds = dbContext.FuelTypes.Select(ft => ft.Id).ToList();
                var transmissionTypesIds = dbContext.TransmissionTypes.Select(gt => gt.Id).ToList();
                var featuresIds = dbContext.Features.Select(vt => vt.Id).ToList();


                var phoneNumbers = new List<string>(new[] { "(670) 309-2183", "(679) 340-0199", "(878) 777-3611", "(695) 214-5297", "(236) 253-7130", "(546) 501-6817", "(667) 421-2466", "(851) 213-5807", "(443) 864-6982", "(261) 377-3425", "(578) 282-9419", "(736) 351-5888", "(914) 914-3479", "(881) 499-7281", "(454) 316-0099", "(695) 540-3684", "(962) 776-8434", "(743) 410-3443", "(265) 930-8419", "(551) 495-0821", "(719) 892-6865", "(712) 265-8480", "(685) 741-6643", "(368) 460-9327", "(277) 489-0443", "(603) 653-6829", "(621) 974-1692", "(939) 616-9905", "(968) 764-9261", "(966) 674-2575", "(271) 675-9175", "(914) 794-8121", "(506) 860-2903", "(998) 846 - 2177", "(672) 259 - 5085", "(323) 546 - 4602", "(224) 874 - 6451", "(703) 567-8344", "(715) 467-1275", "(229) 693-7707", "(518) 747 - 7492", "(777) 431 - 9597", "(958) 268 - 4251", "(359) 994 - 0362", "(995) 341 - 0274", "(679) 718 - 5858", "(421) 986 - 3652", "(261) 677 - 1217", "(567) 714 - 6920", "(458) 200 - 3714", "(918) 391 - 1595", "(876) 353 - 1315", "(718) 939 - 6369", "(536) 621 - 6715", "(491) 459 - 1174", "(939) 260 - 8986", "(289) 298 - 6482", "(626) 752 - 8369", "(757) 642 - 6735", "(302) 800-1794", "(954) 679-8213", "(591) 635-9912", "(302) 545-1067", "(712) 289-3904", "(450) 728-8389", "(320) 521-0603", "(437) 496-3317", "(983) 481-8388", "(538) 762-8202", "(683) 269-7351", "(862) 283-5363", "(451) 431-4749", "(756) 980-6403", "(493) 493-4287", "(995) 731-0489", "(770) 328-4035", "(518) 538-2948", "(302) 488-9289", "(850) 642-0775", "(325) 751-3081", "(972) 200-2723", "(625) 343-3966", "(583) 854-0977", "(858) 884-9278", "(447) 496-6180", "(659) 984-7782", "(468) 346-8208", "(236) 396-9261", "(525) 870-2714", "(749) 857-3482", });
                var description = @"
Lorem ipsum dolor sit amet, consectetur adipiscing elit.Fusce tempor eleifend est et hendrerit. Sed magna ante, dictum ut nisi non, ornare interdum augue. Nulla condimentum a enim et posuere. Donec aliquam tristique arcu quis semper. Quisque aliquam urna malesuada, commodo ex sed, interdum lectus.Sed eu ante sit amet est volutpat ullamcorper. Donec eget vestibulum risus. Integer ut sapien varius, posuere ipsum non, semper magna.Vestibulum quis nisl euismod elit auctor viverra.Mauris lobortis gravida turpis, vitae ullamcorper lectus laoreet quis. Nunc ac consectetur mi. Fusce elementum et nulla vel vulputate. Nulla consequat efficitur convallis. Nunc vehicula erat et sapien interdum, et pharetra mauris tincidunt.Quisque placerat interdum eros in tincidunt.Suspendisse gravida odio ut consequat faucibus.
Nunc sed nisi placerat odio tincidunt posuere.Phasellus sollicitudin metus vel arcu viverra, nec ultricies felis dapibus.Sed pulvinar turpis urna, eget molestie ante faucibus a. In hac habitasse platea dictumst.Fusce quis purus sit amet sapien imperdiet vehicula. Fusce sit amet ullamcorper mauris.Morbi aliquam interdum mi, sed luctus massa tincidunt eu. Vestibulum turpis ligula, suscipit quis leo in, tristique vestibulum ex.Etiam tempus tempor cursus. Aenean a diam quis libero commodo placerat in quis felis. Nunc ut lorem lacus. Duis commodo laoreet quam, vitae iaculis purus. Fusce sollicitudin elit nec sem egestas vestibulum.
Sed metus nisl, interdum ultrices orci laoreet, rutrum sollicitudin libero. Donec in dictum enim, a tempor erat. Aliquam ac lacus turpis. Morbi sed varius felis. Vestibulum non tortor bibendum, tristique purus id, fringilla enim.Praesent cursus laoreet libero. Donec laoreet nunc ac ante placerat tempus.Aenean turpis odio, lobortis eu leo quis, fermentum ultricies ex. Etiam sit amet augue sed arcu vehicula pharetra. Aenean lacinia risus ut nibh interdum tincidunt.
Maecenas ut odio nibh. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Maecenas sed tincidunt leo. Sed viverra at urna eu aliquam. Aenean elementum varius cursus. Aliquam dignissim dolor ullamcorper sapien tempus, non sollicitudin justo laoreet.Suspendisse dictum eget sem eu mollis. Ut nunc ex, mollis id lorem eget, vulputate fermentum elit. Donec sit amet velit lacus.Donec vitae nulla eu nibh porta faucibus.Interdum et malesuada fames ac ante ipsum primis in faucibus.Donec quis lacinia nulla. Nunc placerat maximus lacus. Ut facilisis eu lectus eget facilisis. Mauris sed arcu sed neque finibus vestibulum.
Duis elit nisi, mollis vitae hendrerit eu, aliquet ac metus. Vestibulum placerat sagittis scelerisque. Cras ultrices purus arcu, vitae sagittis turpis placerat ac. Quisque sit amet erat varius, ultricies ante sed, aliquam mi. Suspendisse laoreet ultricies vestibulum. Aliquam fermentum lacus in augue faucibus, ultricies vehicula elit tempor.Etiam lacinia quam sed bibendum sollicitudin. Vestibulum sed nunc malesuada, egestas erat eget, rhoncus leo.Nulla venenatis quam nunc, ac dapibus nisi convallis ac. Duis bibendum mauris eu mauris facilisis viverra.Integer imperdiet neque eget orci lobortis hendrerit.
Mauris varius, est sed eleifend mollis, tellus est tempor sem, id laoreet tellus nulla ac quam.Integer volutpat dui ipsum, finibus bibendum est sodales sit amet.Integer sed venenatis quam. Aliquam maximus eu urna tincidunt condimentum. Sed a dignissim nunc. Morbi aliquam hendrerit suscipit. Fusce at tempus augue. Donec nisi enim, egestas id dapibus ut, scelerisque eu lorem. Phasellus quis tempor turpis. Morbi eleifend pharetra ultricies. Aenean maximus fermentum ante sit amet tincidunt.Maecenas libero tortor, pellentesque sed mattis in, gravida in felis.
Nam egestas erat felis, in malesuada erat interdum sed. Nulla rhoncus et augue eu placerat. Suspendisse molestie luctus justo, nec volutpat lacus sollicitudin sed. In elit nisi, sodales nec magna vitae, pellentesque sodales lacus. Aenean eget orci pharetra felis feugiat faucibus.Pellentesque fringilla lorem sed consequat venenatis. Morbi ut turpis sit amet leo eleifend sodales. Vestibulum in massa ut quam semper laoreet.Aliquam fringilla tincidunt sapien vel euismod. Suspendisse suscipit vestibulum sapien sed pellentesque. Donec vitae laoreet risus, ac maximus ante. Morbi tempor diam quis volutpat laoreet.
Vivamus ullamcorper felis vitae pellentesque placerat. Phasellus consequat quam maximus turpis gravida, nec auctor sapien suscipit.Nam at ante in libero facilisis condimentum vitae sit amet arcu.Quisque ut sapien congue, facilisis turpis ut, commodo odio.In dapibus neque in mi luctus accumsan.Donec est tellus, malesuada a eleifend ac, consequat at mi. Phasellus iaculis tincidunt ex, in condimentum magna luctus ac. In fringilla efficitur purus vel cursus. Nullam finibus massa vel sodales eleifend. Pellentesque semper non dolor vel auctor.
Vivamus efficitur erat mi, et sollicitudin enim auctor a. Mauris vehicula lectus pharetra ligula mollis, vulputate bibendum ipsum volutpat.In malesuada ipsum eget augue pharetra venenatis.Quisque a est nulla. Fusce tincidunt rutrum purus non tincidunt. Vivamus eget metus rhoncus, interdum est at, luctus arcu.Curabitur aliquam pellentesque nisi, eu convallis lectus efficitur id. Duis aliquam ut urna dignissim aliquam. Morbi interdum purus dui. Lorem ipsum dolor sit amet, consectetur adipiscing elit.Sed egestas in elit quis laoreet.Phasellus aliquam congue sollicitudin. Etiam quis tellus semper est tincidunt tempor ac id nibh. Maecenas porta nec lorem condimentum finibus.
Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Suspendisse quis ultricies ex. Nullam vehicula tellus a dignissim pellentesque. Aliquam at cursus tortor, in viverra turpis. Maecenas lacinia est nisl, at tincidunt augue sollicitudin eu. Aenean bibendum a nunc eu semper. Proin sit amet lectus diam.Curabitur quis erat faucibus, consectetur libero vitae, fermentum magna.Nulla volutpat condimentum neque, sed pellentesque nulla pulvinar eu. ";

                var ads = new List<Ad>();
                foreach (var user in usersIds)
                {
                    for (int manufacturerIndex = 0; manufacturerIndex < manufacturers.Count; manufacturerIndex++)
                    {
                        //var model = manufacturers[manufacturerIndex].Models.Take(10).ToList();
                        var model = manufacturers[manufacturerIndex].Models.ToList();
                        foreach (var vehicleModel in model)
                        {
                            for (int vehicleCount = 0; vehicleCount < random.Next(1, 22); vehicleCount++)
                            {
                                var phone = phoneNumbers[random.Next(1, phoneNumbers.Count)];

                                var vehicleFeatures = new List<VehicleFeature>();
                                for (int f = 1; f <= random.Next(1, featuresIds.Count); f++)
                                {
                                    var feature = new VehicleFeature
                                    {
                                        FeatureId = f,
                                    };

                                    vehicleFeatures.Add(feature);
                                }

                                var vehicle = new Vehicle
                                {
                                    ManufacturerId = manufacturerIndex + 1,
                                    IsDeleted = false,
                                    ModelId = vehicleModel.Id,
                                    Condition = (ConditionType)random.Next(0, 1),
                                    Description = description,
                                    Engine = "Lorem ipsum dolor sit amet.",
                                    Features = vehicleFeatures,
                                    EngineHorsePower = random.Next(10, 1000),
                                    FuelConsumption = random.Next(4, 30),
                                    YearOfProduction = random.Next(1990, DateTime.UtcNow.Year),
                                    TotalMileage = random.Next(1, 300000),
                                    Price = random.Next(5000, 1000000),
                                    FuelTypeId = fuelTypesIds[random.Next(0, fuelTypesIds.Count)],
                                    TransmissionTypeId = transmissionTypesIds[random.Next(0, transmissionTypesIds.Count)],
                                    Pictures = new List<Picture> { new Picture { Path = GlobalConstants.DefaultPicturePath } },
                                };

                                var ad = new Ad
                                {
                                    CreationDate = DateTime.UtcNow,
                                    PhoneNumber = phone,
                                    UserId = user,
                                    Vehicle = vehicle,
                                    IsDeleted = false,
                                    IsReported = false,
                                };

                                ads.Add(ad);
                            }
                        }
                    }
                }

                using (var scope = CreateTransactionScope(TimeSpan.FromHours(1)))
                {
                    try
                    {
                        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                        int count = 0;
                        var startTime = DateTime.Now;
                        foreach (var entityToInsert in ads)
                        {
                            ++count;
                            dbContext = AddToContext(dbContext, entityToInsert, count, 100, true, ads.Count, startTime);
                        }
                    }
                    finally
                    {
                        dbContext?.Dispose();
                    }

                    scope.Complete();
                }
            }
        }

        private static void SetTransactionManagerField(string fieldName, object value)
        {
            typeof(TransactionManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
        }

        private static TransactionScope CreateTransactionScope(TimeSpan timeout)
        {
            SetTransactionManagerField("_cachedMaxTimeout", true);
            SetTransactionManagerField("_maximumTimeout", timeout);
            return new TransactionScope(TransactionScopeOption.RequiresNew, timeout);
        }

        private static CarDealerDbContext AddToContext<T>(CarDealerDbContext context,
            T entity, int count, int commitCount, bool recreateContext, int totalElements, DateTime startTime) where T : class
        {
            Task.Run(async () =>
            {
                await context.Set<T>().AddRangeAsync(entity);
            }).Wait();

            if (count % commitCount == 0)
            {
                Task.Run(async () =>
                {
                    await context.SaveChangesAsync();
                }).Wait();
                if (recreateContext)
                {
                    var percent = ((double)count / totalElements) * 100;
                    context.Dispose();
                    context = new CarDealerDbContext();
                    if (count == 500)
                    {
                        Console.WriteLine("Data is being seeded... Please wait!");
                    }
                    if (count % 3000 == 0)
                    {
                        var timeSpent = DateTime.Now - startTime;
                        var percentForCalculatingRemainingTime = (double)count / totalElements;
                        var totalTime = (1.0 / percentForCalculatingRemainingTime) * timeSpent;
                        var remainingTime = totalTime - timeSpent;

                        if (remainingTime.Hours > 0)
                        {
                            Console.WriteLine("Completed: {0:f2}%. Remaining time: {1:%h} hours {1:%m} minutes and {1:%s} seconds.", percent, remainingTime);
                        }
                        if (remainingTime.Hours == 0 && remainingTime.Minutes > 1 && remainingTime.Seconds > 1)
                        {
                            Console.WriteLine("Completed: {0:f2}%. Remaining time: {1:%m} minutes and {1:%s} seconds.", percent, remainingTime);
                        }

                        switch (remainingTime.Minutes)
                        {
                            case 1 when (remainingTime.Hours == 0 && remainingTime.Seconds > 1):
                                Console.WriteLine("Completed: {0:f2}%. Remaining time: {1:%m} minute and {1:%s} seconds.",
                                    percent, remainingTime);
                                break;
                            case 0 when (remainingTime.Hours == 0 && remainingTime.Seconds > 1):
                                Console.WriteLine("Completed: {0:f2}%. Remaining time: {1:%s} seconds.", percent,
                                    remainingTime);
                                break;
                        }

                        if (remainingTime.Seconds == 1 && remainingTime.Hours == 0 && remainingTime.Minutes == 0)
                        {
                            Console.WriteLine("Completed: {0:f2}%. Remaining time: {1:%s} second.", percent, remainingTime);
                        }

                    }
                    context.ChangeTracker.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        private static void SeedManufacturersWithModels(CarDealerDbContext dbContext)
        {
            if (!dbContext.Manufacturers.Any())
            {
                var manufacturers = File.ReadAllText(WebConstants.ManufacturersPath);

                var deserializedManufacturersWithModels =
                    JsonConvert.DeserializeObject<ManufacturerDto[]>(manufacturers);

                var allManufacturers = new List<Manufacturer>();
                foreach (var manufacturerDto in deserializedManufacturersWithModels)
                {
                    var manufacturer = new Manufacturer
                    {
                        Name = manufacturerDto.Name,
                        Models = manufacturerDto
                            .Models
                            .Select(modelName => new Model
                            {
                                Name = modelName
                            })
                            .ToList()
                    };

                    allManufacturers.Add(manufacturer);
                }

                dbContext.AddRange(allManufacturers);
                dbContext.SaveChanges();
            }
        }

        private static void SeedFuelTypes(CarDealerDbContext dbContext)
        {
            if (!dbContext.FuelTypes.Any())
            {
                var fuelTypes = File.ReadAllText(WebConstants.FuelTypesPath);

                var deserializedFuelTypes = JsonConvert.DeserializeObject<FuelType[]>(fuelTypes);

                dbContext.FuelTypes.AddRange(deserializedFuelTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedTransmissionTypes(CarDealerDbContext dbContext)
        {
            if (!dbContext.TransmissionTypes.Any())
            {
                var transmissions = File.ReadAllText(WebConstants.TransmissionTypesPath);

                var deserializedTransmissionTypes = JsonConvert.DeserializeObject<TransmissionType[]>(transmissions);

                dbContext.TransmissionTypes.AddRange(deserializedTransmissionTypes);
                dbContext.SaveChanges();
            }
        }

        private static void SeedFeatures(CarDealerDbContext dbContext)
        {
            if (!dbContext.Features.Any())
            {
                var features = File.ReadAllText(WebConstants.FeaturesPath);

                var deserializedFeatures = JsonConvert.DeserializeObject<Feature[]>(features);

                dbContext.Features.AddRange(deserializedFeatures);
                dbContext.SaveChanges();
            }
        }

        private static void SeedUsers(UserManager<User> userManager, CarDealerDbContext dbContext)
        {
            if (dbContext.Users.Count() == 2)
            {
                var users = File.ReadAllText(WebConstants.UsersPath);

                var deserializedUsers = JsonConvert.DeserializeObject<User[]>(users);

                Task
                    .Run(async () =>
                    {
                        foreach (var user in deserializedUsers)
                        {
                            user.EmailConfirmed = true;

                            await userManager.CreateAsync(user, "test123");
                        }
                    })
                    .Wait();
            }
        }

        private static void SeedDefaultRoles(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            Task
                .Run(async () =>
                {
                    var adminRoleName = WebConstants.AdministratorRole;
                    var seniorAdminRoleName = WebConstants.SeniorAdministratorRole;

                    var roles = new[]
                    {
                        adminRoleName,
                        seniorAdminRoleName,
                    };

                    foreach (var role in roles)
                    {
                        var roleExist = await roleManager.RoleExistsAsync(role);

                        if (!roleExist)
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }

                    await RegisterAdminUser(userManager, adminRoleName);
                    await RegisterAdminUser(userManager, seniorAdminRoleName);
                })
                .Wait();
        }

        private static async Task RegisterAdminUser(UserManager<User> userManager, string adminRoleName)
        {
            var adminEmail = WebConstants.AdministratorEmail;
            var seniorAdminEmail = WebConstants.SeniorAdministratorEmail;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            var seniorUser = await userManager.FindByEmailAsync(seniorAdminEmail);

            if (adminRoleName == "Administrator")
            {
                if (adminUser == null)
                {
                    adminUser = new User
                    {
                        EmailConfirmed = true,
                        Email = adminEmail,
                        UserName = adminEmail,
                    };

                    await userManager.CreateAsync(adminUser, WebConstants.AdministratorPassword);

                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                }
            }
            else if (adminRoleName == "Senior Administrator")
            {
                if (seniorUser == null)
                {
                    seniorUser = new User
                    {
                        EmailConfirmed = true,
                        Email = seniorAdminEmail,
                        UserName = seniorAdminEmail,
                    };

                    await userManager.CreateAsync(seniorUser, WebConstants.SeniorAdministratorPassword);

                    await userManager.AddToRoleAsync(seniorUser, adminRoleName);
                }
            }

        }

        #endregion
    }
}

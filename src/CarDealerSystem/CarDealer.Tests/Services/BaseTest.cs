namespace CarDealer.Tests.Services
{
    using System;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseTest
    {
        protected BaseTest()
        {
            TestSetup.InitializeMapper();
        }

        protected CarDealerDbContext DatabaseInstance
        {
            get
            {
                var options = new DbContextOptionsBuilder<CarDealerDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .EnableSensitiveDataLogging()
                    .Options;

                return new CarDealerDbContext(options);
            }
        }
    }
}

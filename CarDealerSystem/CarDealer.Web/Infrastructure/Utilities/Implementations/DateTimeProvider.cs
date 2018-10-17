namespace CarDealer.Web.Infrastructure.Utilities.Implementations
{
    using System;
    using Interfaces;

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTime() => DateTime.UtcNow;
    }
}

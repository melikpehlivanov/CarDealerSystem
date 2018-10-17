namespace CarDealer.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public abstract class BaseService
    {
        private const string EntityValidationErrorMessage = "Entity validation failed.";
        protected readonly CarDealerDbContext db;

        protected BaseService(CarDealerDbContext db)
        {
            this.db = db;
        }

        protected void ValidateEntityState(object model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            if (!isValid)
            {
                throw new InvalidOperationException(EntityValidationErrorMessage);
            }
        }
    }
}

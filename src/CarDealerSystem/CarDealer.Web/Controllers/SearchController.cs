﻿namespace CarDealer.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Collections;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Services.Interfaces;
    using Services.Models.Vehicle;

    public class SearchController : Controller
    {
        private readonly IVehicleService vehicles;
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleElementService vehicleElements;

        public SearchController(
            IVehicleService vehicles,
            IManufacturerService manufacturers,
            IVehicleElementService vehicleElements)
        {
            this.vehicles = vehicles;
            this.manufacturers = manufacturers;
            this.vehicleElements = vehicleElements;
        }

        public IActionResult Index()
            => this.RedirectToAction(nameof(this.Result), new { manufacturerId = default(int), modelName = string.Empty });

        public async Task<IActionResult> Result(
            int yearOfManufactureMin,
            int manufacturerId,
            string modelName,
            decimal minPrice,
            decimal maxPrice,
            int fuelTypeId,
            int transmissionTypeId,
            int minEngineHorsePower,
            int maximumKilometers,
            int pageIndex = 1)
        {
            var vehicles = this.vehicles
                .Get(yearOfManufactureMin, manufacturerId, modelName, fuelTypeId, transmissionTypeId, minEngineHorsePower, maximumKilometers, minPrice, maxPrice);

            pageIndex = Math.Max(1, pageIndex);
            var totalPages = (int)(Math.Ceiling(vehicles.Count() / (double)WebConstants.SearchResultsPageSize));
            pageIndex = Math.Min(pageIndex, Math.Max(1, totalPages));

            var model = await InitializeSearchModel(
                yearOfManufactureMin,
                manufacturerId,
                modelName,
                minPrice,
                maxPrice,
                minEngineHorsePower,
                maximumKilometers,
                pageIndex,
                vehicles,
                totalPages);

            return View(model);
        }

        private async Task<SearchViewModel> InitializeSearchModel(
            int yearOfManufactureMin,
            int manufacturerId,
            string modelName,
            decimal minPrice,
            decimal maxPrice,
            int minEngineHorsePower,
            int maximumKilometers,
            int pageIndex,
            IQueryable<VehicleSearchServiceModel> vehicles,
            int totalPages)
        {
            var vehiclesToShow = vehicles
                .Skip((pageIndex - 1) * WebConstants.SearchResultsPageSize)
                .Take(WebConstants.SearchResultsPageSize)
                .ToList();

            var results = new PaginatedList<VehicleSearchServiceModel>(vehiclesToShow, pageIndex, totalPages);

            var allManufacturers = await this.manufacturers.AllAsync();
            var availableYears = Enumerable.Range(1990, DateTime.UtcNow.Year - 1990 + 1);
            var allFuelTypes = await this.vehicleElements.GetFuelTypesAsync();
            var allGearingTypes = await this.vehicleElements.GetTransmissionTypesAsync();
            var totalCount = vehicles.Count();

            var model = new SearchViewModel(
                manufacturerId,
                modelName,
                minPrice,
                maxPrice,
                allManufacturers,
                availableYears,
                yearOfManufactureMin,
                allFuelTypes,
                allGearingTypes,
                minEngineHorsePower,
                maximumKilometers,
                results,
                totalCount);
            
            return model;
        }
    }
}

namespace CarDealer.Web.Areas.Ad.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using AutoMapper;
    using CarDealer.Models;
    using CarDealer.Models.BasicTypes;
    using Common.Notifications;
    using Infrastructure.Collections;
    using Infrastructure.Extensions;
    using Infrastructure.Filters;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Caching.Distributed;
    using Models;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using Services.Models.Ad;
    using Services.Models.User;
    using Services.Models.Vehicle;

    public class AdController : BaseController
    {
        private const string ManufacturersCacheKey = "_ManufacturersStoredInCache";
        private const string TransmissionTypesCacheKey = "_TransmissionTypesStoredInCache";
        private const string FuelTypesCacheKey = "_FuelTypesStoredInCache";

        private readonly string webRootPath;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly IDistributedCache cache;
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleElementService vehicleElements;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IAdService ads;
        private readonly IPictureService pictures;

        public AdController(
            IHostingEnvironment hostingEnvironment,
            IDistributedCache cache,
            IDateTimeProvider dateTimeProvider,
            IManufacturerService manufacturers,
            IVehicleElementService vehicleElements,
            UserManager<User> userManager,
            IMapper mapper,
            IAdService ads,
            IPictureService pictures)
        {
            this.webRootPath = hostingEnvironment.WebRootPath;
            this.cache = cache;
            this.dateTimeProvider = dateTimeProvider;
            this.manufacturers = manufacturers;
            this.vehicleElements = vehicleElements;
            this.userManager = userManager;
            this.mapper = mapper;
            this.ads = ads;
            this.pictures = pictures;
        }
        
        public IActionResult Index(string id, string searchTerm, int page = 1)
        {
            page = Math.Max(1, page);
            var allAds = this.ads.GetAllAdsByOwnerId(id);
            if (!allAds.Any())
            {
                return View("NoAdsView");
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allAds = allAds.Where(a => a.Vehicle.FullModelName.ToLower().Contains(searchTerm.ToLower()));
            }

            var totalPages = (int)(Math.Ceiling(allAds.Count() / (double)WebConstants.UsersListPageSize));
            page = Math.Min(page, Math.Max(1, totalPages));

            var adsToShow = allAds
                .Skip((page - 1) * WebConstants.UsersListPageSize)
                .Take(WebConstants.UsersListPageSize)
                .ToList();

            var model = new AdsListingViewModel
            {
                SearchTerm = searchTerm,
                Ads = new PaginatedList<UserAdsListingServiceModel>(adsToShow, page, totalPages)
            };


            return View(model);
        }


        public async Task<IActionResult> Create()
        {
            var model = await InitializeCreationModel();

            return View(model);
        }

        [HttpPost]
        public JsonResult UploadFile(int id)
        {
            try
            {
                var urls = new List<string>();

                foreach (var picture in this.Request.Form.Files)
                {
                    // Process the file to the file system:
                    var extension = GetValidExtension(picture.FileName);
                    var vehiclePictureFolder = $@"vehicle{id}";

                    var dbPath = string.Format(@"/images/vehicles/{0}/{1}", vehiclePictureFolder, GetUniqueFileName(id, extension));

                    var fileProcessingSuccess = ProcessFile(picture, dbPath);

                    if (fileProcessingSuccess)
                    {
                        urls.Add(dbPath);
                    }
                }

                return Json(new { urls });
            }
            catch (Exception)
            {
                this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdCreateViewModel model, List<IFormFile> pictures)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            model.AllFeatures = model
                .AllFeatures
                .Where(f => f.IsChecked)
                .ToList();

            model.UserId = this.userManager.GetUserId(this.User);
            var serviceModel = this.mapper.Map<VehicleCreateServiceModel>(model);
            serviceModel.FeatureIds = new List<int>(model.AllFeatures.Select(f => f.Id));
            var newAdId = await this.ads.CreateAsync(serviceModel);

            try
            {

                foreach (var picture in pictures)
                {
                    // Process the file to the file system:
                    var extension = GetValidExtension(picture.FileName);
                    var vehiclePictureFolder = $@"vehicle{newAdId}";

                    var dbPath = string.Format(@"/images/vehicles/{0}/{1}", vehiclePictureFolder, GetUniqueFileName(newAdId, extension));

                    var fileProcessingSuccess = ProcessFile(picture, dbPath);

                    if (fileProcessingSuccess)
                    {
                        // After successfull processing the image to file system => save it to database:
                        var success = await this.pictures.CreateAsync(dbPath, newAdId);
                        if (!success)
                        {
                            return View(model);
                        }
                    }
                }
            }
            catch
            {
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = newAdId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var ad = await this.ads.GetAsync(id);
            if (ad?.Vehicle == null)
            {
                this.ShowNotification(NotificationMessages.AdDoesNotExist);
                return RedirectToAction(nameof(Index), "Home");
            }
            
            var model = this.mapper.Map<AdDetailsServiceModel, AdDetailsViewModel>(ad);
            model.Vehicle = this.mapper.Map<VehicleDetailsServiceModel, AdDetailsVehicleModel>(ad.Vehicle);
            model.Vehicle.Features = await this.vehicleElements.GetFeaturesByIdAsync(id);

            //var model = new AdDetailsViewModel
            //{
            //    Id = id,
            //    UserEmail = ad.UserEmail,
            //    PhoneNumber = ad.PhoneNumber,
            //    IsReported = ad.IsReported,
            //    Vehicle = new AdDetailsVehicleModel
            //    {
            //        FullModelName = vehicle.FullModelName,
            //        Description = vehicle.Description,
            //        Engine = vehicle.Engine,
            //        EngineHorsePower = vehicle.EngineHorsePower,
            //        FuelConsumption = vehicle.FuelConsumption,
            //        FuelTypeName = vehicle.FuelTypeName,
            //        TransmissionTypeName = vehicle.TransmissionTypeName,
            //        YearOfProduction = vehicle.YearOfProduction,
            //        Price = vehicle.Price,
            //        TotalMileage = vehicle.TotalMileage,
            //        Pictures = vehicle.Pictures,
            //        Features = await this.vehicleElements.GetFeaturesByIdAsync(id)
            //    }
            //};

            return View(model);
        }

        [EnsureOwnership]
        public async Task<IActionResult> Edit(int id)
        {
            var ad = await this.ads.GetForUpdateAsync(id);
            if (ad == null)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            var model = await InitializeEditionModel(this.mapper.Map<AdEditViewModel>(ad));

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdEditViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Urls.Any())
            {
                var success = await this.pictures.RemoveAsync(model.Vehicle.Id);

                if (!success)
                {
                    this.ShowNotification(NotificationMessages.InvalidOperation);
                    return RedirectToAction(nameof(Edit), new { id = model.Id });
                }
            }

            foreach (var pathUrl in model.Urls)
            {
                await this.pictures.CreateAsync(pathUrl, model.Vehicle.Id);
            }

            var serviceModel = this.mapper.Map<AdEditServiceModel>(model);
            serviceModel.Vehicle.FeatureIds = model.Vehicle
                    .AllFeatures
                    .Where(f => f.IsChecked)
                    .Select(f => f.Id)
                    .ToList();

            await this.ads.UpdateAsync(serviceModel);

            this.ShowNotification(NotificationMessages.AdUpdatedSuccessfully, NotificationType.Success);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [EnsureOwnership]
        public async Task<IActionResult> Delete(int id)
        {
            var ad = await this.ads.GetAsync(id);
            if (ad == null)
            {
                this.ShowNotification(NotificationMessages.AdDoesNotExist);
                return RedirectToAction(nameof(Index), "Home");
            }

            var model = this.mapper.Map<AdDetailsViewModel>(ad);
            model.Vehicle.Features = await this.vehicleElements.GetFeaturesByIdAsync(id);

            return View(model);
        }

        [ActionName(nameof(Delete))]
        [HttpPost]
        [EnsureOwnership]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = this.userManager.GetUserId(this.User);
            var success = await this.ads.DeleteAsync(id);
            if (!success)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAction(nameof(Delete), new { id });
            }

            this.ShowNotification(NotificationMessages.AdDeletedSuccessfully, NotificationType.Success);
            return RedirectToAction(nameof(Index), new { id = userId });
        }

        #region privateMethods

        private async Task<AdCreateViewModel> InitializeCreationModel()
        {
            var model = new AdCreateViewModel
            {
                AllManufacturers = await GetAllManufacturersAsync(),
                AllFuelTypes = await GetAllFuelTypesAsync(),
                AllTransmissionTypes = await GetAllTransmissionTypesAsync(),
                AvailableYears = GetAvailableYears(),
                AllFeatures = await this.vehicleElements.GetFeaturesAsync()
            };

            return model;
        }
        
        private async Task<AdEditViewModel> InitializeEditionModel(AdEditViewModel model)
        {
            var checkedFeatures = await this.vehicleElements.GetFeaturesByIdAsync(model.Id);
            var allFeatures = await this.vehicleElements.GetFeaturesAsync();
            allFeatures = new List<Feature>(allFeatures.Select(f => new Feature
            {
                Id = f.Id,
                Name = f.Name,
                IsChecked = f.IsChecked = checkedFeatures.Any(c => c.Id == f.Id)
            }));

            model.Vehicle.AllManufacturers = await GetAllManufacturersAsync();
            model.Vehicle.AllFuelTypes = await GetAllFuelTypesAsync();
            model.Vehicle.AllTransmissionTypes = await GetAllTransmissionTypesAsync();
            model.Vehicle.AvailableYears = GetAvailableYears();
            model.Vehicle.AllFeatures = allFeatures;
            return model;
        }
        
        private async Task<IEnumerable<SelectListItem>> GetAllManufacturersAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(ManufacturersCacheKey);
            if (listFromCache == null)
            {
                var allManufacturers = await this.manufacturers.AllAsync();
                list = allManufacturers.Select(m => new SelectListItem(m.Name.ToString(), m.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.cache.SetSerializableObject(ManufacturersCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private async Task<IEnumerable<SelectListItem>> GetAllTransmissionTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(TransmissionTypesCacheKey);
            if (listFromCache == null)
            {
                var transmissionTypes = await this.vehicleElements.GetTransmissionTypes();
                list = transmissionTypes.Select(x => new SelectListItem(x.Name.ToString(), x.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.cache.SetSerializableObject(TransmissionTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private async Task<IEnumerable<SelectListItem>> GetAllFuelTypesAsync()
        {
            IEnumerable<SelectListItem> list;

            var listFromCache = await this.cache.GetStringAsync(FuelTypesCacheKey);
            if (listFromCache == null)
            {
                var fuelTypes = await this.vehicleElements.GetFuelTypes();
                list = fuelTypes.Select(f => new SelectListItem(f.Name.ToString(), f.Id.ToString()));
                var expiration = TimeSpan.FromDays(WebConstants.StaticElementsCacheExpirationInDays);

                await this.cache.SetSerializableObject(FuelTypesCacheKey, list, expiration);
            }
            else
            {
                list = JsonConvert.DeserializeObject<IEnumerable<SelectListItem>>(listFromCache);
            }

            return list;
        }

        private IEnumerable<SelectListItem> GetAvailableYears()
        {
            return Enumerable
                .Range(1990, this.dateTimeProvider.GetCurrentDateTime().Year - 1990 + 1)
                .Select(y => new SelectListItem(y.ToString(), y.ToString()));
        }

        private bool ProcessFile(IFormFile file, string dbPath)
        {
            var fileFullPath = Path.Combine(this.webRootPath, dbPath.TrimStart('/', '\\'));

            var uploaded = UploadImage(fileFullPath, file);
            if (!uploaded)
            {
                return false;
            }

            return true;
        }

        private bool UploadImage(string fileFullPath, IFormFile file)
        {
            try
            {
                var directoryName = Path.GetDirectoryName(fileFullPath);
                if (directoryName.Length > 0 && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                using (var stream = new FileStream(fileFullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        private string GetValidExtension(string fileName)
        {
            var extension = fileName.Split('.').LastOrDefault()?.ToLower();

            switch (extension)
            {
                case "jpg":
                case "png":
                case "jpeg":
                case "bmp":
                    return extension;
                default:
                    return null;
            }
        }

        private string GetUniqueFileName(int adId, string extension)
        {
            return $"ID_{adId}_{Guid.NewGuid().ToString().Substring(0, 6)}.{extension}";
        }

        #endregion
    }
}

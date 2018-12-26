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
    using Infrastructure.Collections.Interfaces;
    using Infrastructure.Filters;
    using Infrastructure.Utilities.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;
    using Services.Interfaces;
    using Services.Models.Ad;
    using Services.Models.User;
    using Services.Models.Vehicle;

    public class AdController : BaseController
    {
        private const string NoAdsFoundView = "NoAdsView";
        private const string DbPath = "/images/vehicles/{0}/{1}";
        private const string VehiclePictureFolder = "vehicle{0}";

        private readonly string webRootPath;
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly ICache cache;
        private readonly IManufacturerService manufacturers;
        private readonly IVehicleElementService vehicleElements;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IAdService ads;
        private readonly IPictureService pictures;
        private readonly IEmailSender emailSender;

        public AdController(
            IHostingEnvironment hostingEnvironment,
            ICache cache,
            IDateTimeProvider dateTimeProvider,
            IManufacturerService manufacturers,
            IVehicleElementService vehicleElements,
            UserManager<User> userManager,
            IMapper mapper,
            IAdService ads,
            IPictureService pictures,
            IEmailSender emailSender)
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
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> Index(string id, string searchTerm, int page = 1)
        {
            page = Math.Max(1, page);
            var allAds = await this.ads.GetAllAdsByOwnerId(id);
            if (!allAds.Any())
            {
                return View(NoAdsFoundView);
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
        public IActionResult UploadFile(int id)
        {
            try
            {
                var urls = new List<string>();

                foreach (var picture in this.Request.Form.Files)
                {
                    // Process the file to the file system:
                    var extension = GetValidExtension(picture.FileName);
                    var vehiclePictureFolder = string.Format(VehiclePictureFolder, id);
                    var dbPath = string.Format(DbPath, vehiclePictureFolder, GetUniqueFileName(id, extension));

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
            var newAd = await this.ads.CreateAsync(serviceModel);
            try
            {

                foreach (var picture in pictures)
                {
                    // Process the file to the file system:
                    var extension = GetValidExtension(picture.FileName);
                    var vehiclePictureFolder = string.Format(VehiclePictureFolder, newAd.VehicleId);
                    var dbPath = string.Format(DbPath, vehiclePictureFolder, GetUniqueFileName(newAd.VehicleId, extension));
                    
                    var fileProcessingSuccess = ProcessFile(picture, dbPath);

                    if (fileProcessingSuccess)
                    {
                        // After successfull processing the image to file system => save it to database:
                        var success = await this.pictures.CreateAsync(dbPath, newAd.VehicleId);
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

            return RedirectToAction(nameof(Details), new { id = newAd.AdId });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var ad = await this.ads.GetAsync(id);
            if (ad?.Vehicle == null)
            {
                this.ShowNotification(NotificationMessages.AdDoesNotExist);
                return RedirectToHome();
            }

            var model = this.mapper.Map<AdDetailsServiceModel, AdDetailsViewModel>(ad);
            model.Vehicle = this.mapper.Map<VehicleDetailsServiceModel, AdDetailsVehicleModel>(ad.Vehicle);
            model.Vehicle.Features = await this.vehicleElements.GetFeaturesByIdAsync(id);
            
            return View(model);
        }

        [EnsureOwnership(isAdminAllowed: false)]
        public async Task<IActionResult> Edit(int id)
        {
            var ad = await this.ads.GetForUpdateAsync(id);
            if (ad == null)
            {
                return RedirectToHome();
            }

            var model = await InitializeEditionModel(this.mapper.Map<AdEditViewModel>(ad));

            return View(model);

        }

        [HttpPost]
        [EnsureOwnership(isAdminAllowed: false)]
        public async Task<IActionResult> Edit(AdEditViewModel model, int id)
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
                return RedirectToHome();
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
        
        [AllowAnonymous]
        public async Task<IActionResult> ContactOwner(TestDriveViewModel model)
        {
            var receiver = await this.ads.GetAdOwnerEmail(model.ReceiverId);
            if (receiver == null)
            {
                this.ShowNotification(NotificationMessages.InvalidOperation);
                return RedirectToAction(nameof(Details), new { id = model.ReceiverId });
            }
            await this.emailSender.SendEmailAsync(model.Email, receiver, model.Subject, model.Message);

            this.ShowNotification(NotificationMessages.EmailSentSuccessfully, NotificationType.Success);
            return RedirectToAction(nameof(Details), new { id = model.ReceiverId });
        }

        #region privateMethods

        private async Task<AdCreateViewModel> InitializeCreationModel()
        {
            var model = new AdCreateViewModel
            {
                AllManufacturers = await this.cache.GetAllManufacturersAsync(),
                AllFuelTypes = await this.cache.GetAllFuelTypesAsync(),
                AllTransmissionTypes = await this.cache.GetAllTransmissionTypesAsync(),
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

            model.Vehicle.AllManufacturers = await this.cache.GetAllManufacturersAsync();
            model.Vehicle.AllFuelTypes = await this.cache.GetAllFuelTypesAsync();
            model.Vehicle.AllTransmissionTypes = await this.cache.GetAllTransmissionTypesAsync();
            model.Vehicle.AvailableYears = GetAvailableYears();
            model.Vehicle.AllFeatures = allFeatures;
            return model;
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

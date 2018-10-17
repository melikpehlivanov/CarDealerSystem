namespace CarDealer.Services.Implementations.Picture
{
    using System.Linq;
    using System.Threading.Tasks;
    using CarDealer.Models;
    using Data;
    using Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class PictureService : BaseService, IPictureService
    {
        public PictureService(CarDealerDbContext db) 
            : base(db)
        {
        }

        public async Task<bool> CreateAsync(string path, int vehicleId)
        {
            var picture = new Picture
            {
                Path = path,
                VehicleId = vehicleId
            };

            var vehicle = await this.db
                .Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId);
            if (vehicle == null)
            {
                return false;
            }

            try
            {
                await this.db.Pictures.AddAsync(picture);
                vehicle.Pictures.Add(picture);
                this.db.Update(vehicle);

                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(int vehicleId)
        {
            var picture = await this.db
                .Pictures
                .Where(p => p.VehicleId == vehicleId)
                .ToListAsync();

            if (picture == null)
            {
                return false;
            }

            try
            {
                this.db.Pictures.RemoveRange(picture);
                await this.db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

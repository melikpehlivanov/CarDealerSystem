namespace CarDealer.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IPictureService
    {
        Task<bool> CreateAsync(string path, int vehicleId);

        Task<bool> RemoveAsync(int vehicleId);
    }
}

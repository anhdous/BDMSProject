namespace ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

public interface IBloodDriveService 
{
  Task<List<BloodDriveModel>> GetAllBloodDrives();
  Task<List<BloodDriveModel>> GetOpenBloodDrives();
  Task<int> AddBloodDrive(BloodDriveModel model);
  Task<BloodDriveModel> GetBloodDriveById(int Id);
  Task UpdateBloodDrive(BloodDriveModel model);
  Task DeleteBloodDrive(int Id);
}

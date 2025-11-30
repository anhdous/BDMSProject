namespace ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

public interface IBloodDriveService 
{
  Task<List<BloodDriveModel>> GetAllBloodDrives();
  Task<List<BloodDriveModel>> GetOpenBloodDrives();
}

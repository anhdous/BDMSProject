namespace Infrastructure.Services;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
public class BloodDriveService: IBloodDriveService
{
 private readonly IBloodDriveRepository _bloodDriveRepository;
 public BloodDriveService(IBloodDriveRepository bloodDriveRepository ){
   _bloodDriveRepository = bloodDriveRepository;
   }
   public async Task<List<BloodDriveModel>> GetAllBloodDrives()
  {
    var bds = await _bloodDriveRepository.GetAllBloodDrives();
    return bds;
  }
  public async Task<List<BloodDriveModel>> GetOpenBloodDrives()
  {
    var bds = await _bloodDriveRepository.GetOpenBloodDrives();
    return bds;
  }
}

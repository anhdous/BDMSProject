namespace ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Models;
public interface IBloodDriveRepository
{
   // CRUD methods
   // get all blood drives from database
   Task<List<BloodDriveModel>> GetAllBloodDrives();
   Task<List<BloodDriveModel>> GetOpenBloodDrives();
  
 }


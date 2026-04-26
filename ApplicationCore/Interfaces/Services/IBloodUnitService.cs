namespace ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

public interface IBloodUnitService 
{
  Task<int> AddRequest (BloodRequestModel model);
  Task<List<BloodRequestModel>> GetRequestsByHospital(int hospitalId);
  Task<List<BloodRequestModel>> GetPendingRequests();
  Task<List<BloodUnitModel>> GetAvailableUnits(string bloodType, string rh, string component);
  Task AssignUnitsToRequest(int requestId, List<int> unitIds);
  Task<BloodRequestModel?> GetRequestById(int id);
  Task<int> CountAssignedUnits(int requestId);
  Task UpdateRequestStatus(int requestId);
}

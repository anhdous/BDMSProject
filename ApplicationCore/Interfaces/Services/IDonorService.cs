namespace ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

public interface IDonorService 
{
  Task<List<DonorModel>> GetAllDonors();
  Task<List<MedicalHistoryListModel>> GetMedicalHistory(int Id);
}

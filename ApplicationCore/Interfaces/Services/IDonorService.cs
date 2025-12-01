namespace ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

public interface IDonorService 
{
  Task<List<DonorModel>> GetAllDonors();
  Task<List<MedicalHistoryListModel>> GetMedicalHistory(int Id);
  Task<int> AddAppointment(ScheduleAppointmentModel m);
  Task<List<DonationHistoryListModel>> GetDonationHistory(int Id);
  Task UpdateMedicalHistory(MedicalHistoryListModel model);
}

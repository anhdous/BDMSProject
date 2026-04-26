namespace ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

public interface IDonorService 
{
  Task<List<DonorModel>> GetAllDonors(string search, string bloodType);
 Task<DonorModel> GetDonorById(int id);
  Task<List<MedicalHistoryListModel>> GetMedicalHistory(int Id);
  Task<int> AddAppointment(ScheduleAppointmentModel m);
  Task<List<DonationHistoryListModel>> GetDonationHistory(int Id);
  Task UpdateMedicalHistory(MedicalHistoryListModel model);
  Task<List<AppointmentListModel>> GetAppointmentsForDonor(int donorId);
  Task<AppointmentListModel> GetAppointmentsById(int Id);
  Task UpdateAppointment(ScheduleAppointmentModel model);
  Task DeleteAppointment(int Id);
}

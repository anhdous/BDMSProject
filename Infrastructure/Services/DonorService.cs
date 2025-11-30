namespace Infrastructure.Services;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
public class DonorService: IDonorService
{
 private readonly IDonorRepository _donorRepository;
 public DonorService(IDonorRepository donorRepository ){
   _donorRepository = donorRepository;
   }
   public async Task<List<DonorModel>> GetAllDonors()
  {
    var donors = await _donorRepository.GetAllDonors();
    return donors;
  }
     public async Task<List<MedicalHistoryListModel>> GetMedicalHistory(int Id)
  {
    var historylist = await _donorRepository.GetMedicalHistory(Id);
    return historylist;
  }
    public async Task<int> AddAppointment(ScheduleAppointmentModel m)
  {
    var res = await _donorRepository.AddAppointment(m);
    return res;
  }
  public async Task<List<DonationHistoryListModel>> GetDonationHistory(int Id)
  {
    var historylist = await _donorRepository.GetDonationHistory(Id);
    return historylist;
  }
}

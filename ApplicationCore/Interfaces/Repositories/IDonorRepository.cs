namespace ApplicationCore.Interfaces.Repositories;
using System.Collections.Generic;
using ApplicationCore.Models;
public interface IDonorRepository
{
   Task<List<DonorModel>> GetAllDonors();
  Task<List<MedicalHistoryListModel>> GetMedicalHistory(int Id);
 }

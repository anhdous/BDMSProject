using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Services;

using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Dapper;

public class DonorService : IDonorService
{
  private readonly IDonorRepository _donorRepository;
  private readonly IConfiguration _cfg;
  public DonorService(IDonorRepository donorRepository, IConfiguration cfg)
  {
    _donorRepository = donorRepository;
    _cfg = cfg;
  }
  public async Task<List<DonorModel>> GetAllDonors(string search, string bloodType)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    var sql = @"
        SELECT 
            d.DonorID,
            d.FullName,
            d.BloodType,
            d.RhFactor,
            d.Phone,
            d.Email,
            MAX(do.DonationDate) AS LastDonationDate
        FROM dbo.Donor d
        LEFT JOIN dbo.Donation do ON d.DonorID = do.DonorID
        WHERE (@Search IS NULL OR d.FullName LIKE '%' + @Search + '%')
          AND (@BloodType IS NULL OR (LTRIM(RTRIM(d.BloodType))+ d.RhFactor) = @BloodType)
        GROUP BY 
            d.DonorID, d.FullName, d.BloodType, d.RhFactor,
            d.Phone, d.Email
        ORDER BY d.FullName;
    ";

    var donors = await conn.QueryAsync<DonorModel>(sql, new
    {
      Search = string.IsNullOrEmpty(search) ? null : search,
      BloodType = string.IsNullOrEmpty(bloodType) ? null : bloodType
    });
    return donors.ToList();
  }

  public async Task<DonorModel> GetDonorById(int id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    return await conn.QueryFirstOrDefaultAsync<DonorModel>(
        "SELECT * FROM Donor WHERE DonorID = @Id",
        new { Id = id });
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
  public async Task UpdateMedicalHistory(MedicalHistoryListModel model)
  {
    await _donorRepository.UpdateMedicalHistory(model);
  }
  public async Task<List<AppointmentListModel>> GetAppointmentsForDonor(int donorId)
  {
    var appointments = await _donorRepository.GetAppointmentsForDonor(donorId);
    return appointments;
  }

  public async Task<AppointmentListModel> GetAppointmentsById(int Id)
  {
    var a = await _donorRepository.GetAppointmentsById(Id);
    return a;
  }
  public async Task UpdateAppointment(ScheduleAppointmentModel model)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    const string sql = @"
      UPDATE dbo.Appointment
      SET Location = @Location,
          DateTime = @DateTime,
          BDID = @BDID
      WHERE AppointmentID= @AppointmentID;";
    var parameters = new
    {
      Location = model.Location,
      DateTime = model.Date.Value.ToDateTime(model.Time.Value),
      BDID = model.BDID,
      AppointmentID = model.AppointmentID
    };
    await conn.ExecuteAsync(sql, parameters);
  }
  public async Task DeleteAppointment(int Id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    const string sql = @"
      DELETE FROM dbo.Appointment
      WHERE AppointmentID= @AppointmentID;";
    var parameters = new
    {
      AppointmentID = Id
    };
    await conn.ExecuteAsync(sql, parameters);
  }

}

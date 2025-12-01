using ApplicationCore.Models;
using ApplicationCore.Interfaces.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Repositories { 
  public class DonorRepository : IDonorRepository
  {
  private readonly IConfiguration _cfg;
  public DonorRepository(IConfiguration cfg) {
    _cfg = cfg;
    }
  public async Task<List<DonorModel>> GetAllDonors()
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var donors = await conn.QueryAsync<DonorModel>(
        "SELECT * FROM dbo.Donor ORDER BY FullName Asc");
    return donors.ToList();
    }
  
  public async Task<List<MedicalHistoryListModel>> GetMedicalHistory(int Id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var donors = await conn.QueryAsync<MedicalHistoryListModel>(
        @"SELECT * FROM dbo.MedicalHistory 
        WHERE DonorID = @Id 
        ORDER BY DiagnosisDate Desc", new { Id });
    return donors.ToList();
  }

  public async Task<int> AddAppointment(ScheduleAppointmentModel m)
  {
    DateTime aptDateTime = m.Date.ToDateTime(m.Time);
    const string sql = @"
        INSERT INTO dbo.Appointment ( DateTime, Location, Status, DonorID, BDID)
        VALUES (@DateTime, @Location, 'Scheduled', @DonorID, @BDID);
        SELECT CAST(SCOPE_IDENTITY() AS int);";
    var parameters = new
    {
      Location = m.Location,
      DateTime = aptDateTime,
      DonorID = m.DonorID,
      BDID = m.BDID
    };
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    return await conn.ExecuteScalarAsync<int>(sql, parameters);
  }

  public async Task<List<DonationHistoryListModel>> GetDonationHistory(int Id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var dh = await conn.QueryAsync<DonationHistoryListModel>(
        @"SELECT DonationDate, [Location], CollectedVolume, ht.FullName AS StaffName 
        FROM dbo.Donation d 
        JOIN HospitalStaff ht ON ht.StaffID = d.StaffID 
        WHERE DonorId = @Id 
        ORDER BY DonationDate Desc", new { Id });
    return dh.ToList();
  }

  public async Task UpdateMedicalHistory(MedicalHistoryListModel model)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    const string sql = @"
      UPDATE dbo.MedicalHistory
      SET ConditionName = @ConditionName,
          DiagnosisDate = @DiagnosisDate,
          RecoveryDate = @RecoveryDate,
          Status = @Status
      WHERE MID = @MID;";
    var parameters = new
    {
      ConditionName = model.ConditionName,
      DiagnosisDate = model.DiagnosisDate,
      RecoveryDate = model.RecoveryDate,
      Status = model.Status,
      MID = model.MID
    };
    await conn.ExecuteAsync(sql, parameters);
  }
  }
}
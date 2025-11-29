using ApplicationCore.Models;
using ApplicationCore.Interfaces.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Repositories{
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
        "SELECT * FROM dbo.MedicalHistory WHERE DonorId = @Id ORDER BY DiagnosisDate Desc", new { Id });
    return donors.ToList();
  }
  }
}
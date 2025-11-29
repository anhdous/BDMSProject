using ApplicationCore.Models;
using ApplicationCore.Interfaces.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace Infrastructure.Repositories{
  public class BloodDriveRepository : IBloodDriveRepository
  {
  private readonly IConfiguration _cfg;
  public BloodDriveRepository(IConfiguration cfg) {
    _cfg = cfg;
    }
  public async Task<List<BloodDriveModel>> GetAllBloodDrives()
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var bds = await conn.QueryAsync<BloodDriveModel>(
        "SELECT * FROM dbo.BloodDrive ORDER BY StartDate Desc");
    return bds.ToList();
    }
  }
}
namespace Infrastructure.Services;

using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class BloodDriveService : IBloodDriveService
{
  private readonly IConfiguration _cfg;
  private readonly IBloodDriveRepository _bloodDriveRepository;
  public BloodDriveService(IBloodDriveRepository bloodDriveRepository, IConfiguration cfg)
  {
    _bloodDriveRepository = bloodDriveRepository;
    _cfg = cfg;
  }
  public async Task<List<BloodDriveModel>> GetAllBloodDrives()
  {
    var bds = await _bloodDriveRepository.GetAllBloodDrives();
    return bds;
  }
  public async Task<List<BloodDriveModel>> GetOpenBloodDrives()
  {
    var bds = await _bloodDriveRepository.GetOpenBloodDrives();
    return bds;
  }
  public async Task<int> AddBloodDrive(BloodDriveModel m)
  {
    const string sql = @"
        INSERT INTO dbo.BloodDrive (Title, Location, StartDate, EndDate, Organizer)
        VALUES (@Title, @Location, @StartDate, @EndDate, @Organizer);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

    var parameters = new
    {
      Title = m.Title,
      Location = m.Location,
      StartDate = m.StartDate,
      EndDate = m.EndDate,
      Organizer = m.Organizer
    };

    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    return await conn.ExecuteScalarAsync<int>(sql, parameters);
  }

  public async Task<BloodDriveModel> GetBloodDriveById(int Id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var bd = await conn.QueryAsync<BloodDriveModel>(
     @"SELECT *
        FROM dbo.BloodDrive
        WHERE BDID = @Id"
     , new { Id });
    return bd.FirstOrDefault();
  }

  public async Task UpdateBloodDrive(BloodDriveModel model)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        UPDATE dbo.BloodDrive
        SET Title = @Title,
            Location = @Location,
            StartDate = @StartDate,
            EndDate = @EndDate,
            Organizer = @Organizer
        WHERE BDID = @BDID;";

    var parameters = new
    {
      model.Title,
      model.Location,
      model.StartDate,
      model.EndDate,
      model.Organizer,
      model.BDID
    };

    await conn.ExecuteAsync(sql, parameters);
  }
  public async Task DeleteBloodDrive(int id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        DELETE FROM dbo.BloodDrive
        WHERE BDID = @BDID;";

    await conn.ExecuteAsync(sql, new { BDID = id });
  }

}

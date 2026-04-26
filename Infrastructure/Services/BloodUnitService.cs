using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class BloodUnitService : IBloodUnitService
{
  private readonly IConfiguration _cfg;
  public BloodUnitService(IConfiguration cfg)
  {
    _cfg = cfg;
  }
  public async Task<int> AddRequest(BloodRequestModel m)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        INSERT INTO dbo.BloodRequest
        (
            RequestDate,
            BloodType,
            RhFactor,
            ComponentType,
            Quantity,
            UrgencyLevel,
            Status,
            HospitalID
        )
        VALUES
        (
            @RequestDate,
            @BloodType,
            @RhFactor,
            @ComponentType,
            @Quantity,
            @UrgencyLevel,
            @Status,
            @HospitalID
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);
    ";

    var id = await conn.ExecuteScalarAsync<int>(sql, new
    {
      m.RequestDate,
      m.BloodType,
      m.RhFactor,
      m.ComponentType,
      m.Quantity,
      m.UrgencyLevel,
      m.Status,
      m.HospitalID
    });
    return id;

  }
  public async Task<List<BloodRequestModel>> GetRequestsByHospital(int hospitalId)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        SELECT 
            RequestID,
            RequestDate,
            BloodType,
            RhFactor,
            ComponentType,
            Quantity,
            UrgencyLevel,
            Status,
            HospitalID
        FROM dbo.BloodRequest
        WHERE HospitalID = @HospitalID
        ORDER BY RequestDate DESC;
    ";

    var requests = await conn.QueryAsync<BloodRequestModel>(sql, new
    {
      HospitalID = hospitalId
    });
    return requests.ToList();
  }

  public async Task<List<BloodRequestModel>> GetPendingRequests()
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        SELECT r.*,
              (SELECT COUNT(*)
              FROM BloodUnit b
              WHERE b.BloodType = r.BloodType
                AND b.RhFactor = r.RhFactor
                AND b.ComponentType = r.ComponentType
                AND b.Status = 'Available'
              ) AS AvailableUnits
        FROM BloodRequest r
        WHERE r.Status in ('Pending','Partially Fulfilled')
        ORDER BY
          CASE WHEN (
                    SELECT COUNT(*) 
                    FROM BloodUnit u
                    WHERE 
                        u.BloodType = r.BloodType
                        AND u.RhFactor = r.RhFactor
                        AND u.ComponentType = r.ComponentType
                        AND u.Status = 'Available'
                ) > 0 THEN 0 ELSE 1 END,
          CASE r.UrgencyLevel
              WHEN 'Critical' THEN 1
              WHEN 'High' THEN 2
              WHEN 'Medium' THEN 3
              WHEN 'Low' THEN 4
              ELSE 5
          END,
          r.RequestDate ASC;";

    var requests = await conn.QueryAsync<BloodRequestModel>(sql);
    return requests.ToList();
  }

  public async Task<List<BloodUnitModel>> GetAvailableUnits(string bloodType, string rh, string component)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        SELECT *
        FROM BloodUnit
        WHERE Status = 'Available'
          AND BloodType = @BloodType
          AND RhFactor = @RhFactor
          AND ComponentType = @ComponentType
        ORDER BY CollectionDate;
    ";

    var units = await conn.QueryAsync<BloodUnitModel>(sql, new
    {
      BloodType = bloodType,
      RhFactor = rh,
      ComponentType = component
    });
    return units.ToList();
  }

  public async Task AssignUnitsToRequest(int requestID, List<int> unitIds)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    foreach (var unitId in unitIds)
    {
      await conn.ExecuteAsync(@"
            UPDATE BloodUnit
            SET RequestID = @RequestID,
                Status = 'Issued'
            WHERE UnitID = @UnitID
        ", new { RequestID = requestID, UnitID = unitId });
    }
    await UpdateRequestStatus(requestID);
  }

  public async Task UpdateRequestStatus(int requestId)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    // how many units assigned
    var assigned = await conn.ExecuteScalarAsync<int>(@"
        SELECT COUNT(*) 
        FROM BloodUnit 
        WHERE RequestID = @RequestID
    ", new { RequestID = requestId });

    // get requested quantity
    var request = await conn.QueryFirstOrDefaultAsync<BloodRequestModel>(@"
        SELECT Quantity
        FROM BloodRequest
        WHERE RequestID = @RequestID
    ", new { RequestID = requestId });

    if (request == null) return;

    string status;

    if (assigned == 0)
      status = "Pending";
    else if (assigned < request.Quantity)
      status = "Partially Fulfilled";
    else
      status = "Completed";

    await conn.ExecuteAsync(@"
        UPDATE BloodRequest
        SET Status = @Status
        WHERE RequestID = @RequestID
    ", new { Status = status, RequestID = requestId });
  }

  public async Task<BloodRequestModel?> GetRequestById(int id)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        SELECT 
            RequestID,
            RequestDate,
            BloodType,
            RhFactor,
            ComponentType,
            Quantity,
            UrgencyLevel,
            Status,
            HospitalID
        FROM dbo.BloodRequest
        WHERE RequestID = @RequestID;
    ";

    return await conn.QueryFirstOrDefaultAsync<BloodRequestModel>(sql, new
    {
      RequestID = id
    });
  }

  public async Task<int> CountAssignedUnits(int requestId)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    return await conn.ExecuteScalarAsync<int>(@"
        SELECT COUNT(*) 
        FROM BloodUnit
        WHERE RequestID = @RequestID
    ", new { RequestID = requestId });
  }

}
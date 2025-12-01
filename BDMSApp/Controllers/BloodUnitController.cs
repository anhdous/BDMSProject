using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
namespace BDMSApp.Controllers;


public class BloodUnitController: Controller
{
  private readonly IConfiguration _cfg;

  public BloodUnitController( IConfiguration config)
  {
      _cfg = config;
  }
  [HttpGet]
  [Authorize(Roles = "Hospital Staff")]
  public IActionResult Inventory()
  {
      var model = new BloodInventoryFilterModel();
      return View(model);
  }
  
  [HttpPost]
  [Authorize(Roles = "Hospital Staff")]
  public async Task<IActionResult> Inventory(BloodInventoryFilterModel model)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var results = await conn.QueryAsync<BloodInventoryRecord>(
        "sp_CheckBloodProductAvailability",
        new
        {
            ComponentType = model.ComponentType,
            BloodType     = model.BloodType,
            RhFactor      = model.RhFactor,
            Status        = model.Status
        },
        commandType: CommandType.StoredProcedure
    );

    model.Results = results.ToList();
    return View(model);
  }
}
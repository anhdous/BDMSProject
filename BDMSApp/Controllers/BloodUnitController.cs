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


public class BloodUnitController : Controller
{
  private readonly IConfiguration _cfg;
  private readonly IBloodUnitService _bloodUnitService;

  public BloodUnitController(IConfiguration config, IBloodUnitService bloodUnitService)
  {
    _cfg = config;
    _bloodUnitService = bloodUnitService;

  }
  [HttpGet]
  [Authorize(Roles = "Hospital Staff, Admin")]
  public IActionResult Inventory()
  {
    var model = new BloodInventoryFilterModel();
    return View(model);
  }

  [HttpPost]
  [Authorize(Roles = "Hospital Staff, Admin")]
  public async Task<IActionResult> Inventory(BloodInventoryFilterModel model)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var results = await conn.QueryAsync<BloodInventoryRecord>(
        "sp_CheckBloodProductAvailability",
        new
        {
          ComponentType = model.ComponentType,
          BloodType = model.BloodType,
          RhFactor = model.RhFactor,
          Status = model.Status
        },
        commandType: CommandType.StoredProcedure
    );

    model.Results = results.ToList();
    model.HasSearched = true;
    return View(model);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public IActionResult UsageReport()
  {
    var model = new UsageReportFilterModel();
    model.StartDate = DateTime.Today.AddYears(-1);
    model.EndDate = DateTime.Today;
    return View(model);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> UsageReport(UsageReportFilterModel model)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var results = await conn.QueryAsync<UsageReportDetailModel>(
        "sp_BloodUsageTrend",
        new
        {
          StartDate = model.StartDate,
          EndDate = model.EndDate,
          ComponentType = model.ComponentType,
          BloodType = model.BloodType,
          RhFactor = model.RhFactor,
        },
        commandType: CommandType.StoredProcedure
    );

    model.Results = results.ToList();
    model.HasSearched = true;
    return View(model);
  }

  [Authorize(Roles = "Hospital Staff")]
  [HttpGet]
  public IActionResult RequestUnits()
  {
    return View();
  }
  [Authorize(Roles = "Hospital Staff")]
  [HttpPost]
  public async Task<IActionResult> CreateRequest(BloodRequestModel model)
  {
    if (model.Quantity <= 0)
    {
      ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
    }
    // 1. Validate form
    if (!ModelState.IsValid)
    {
      return View("RequestUnits", model);
    }

    // 2. Get HospitalID from logged-in user (claims)
    var hospitalClaim = User.FindFirst("HospitalID");

    if (hospitalClaim == null)
    {
      ModelState.AddModelError("", "Unable to determine hospital.");
      return View("RequestUnits", model);
    }

    int hospitalId = int.Parse(hospitalClaim.Value);

    // 3. Set system-generated fields
    model.HospitalID = hospitalId;
    model.RequestDate = DateTime.Now;
    model.Status = "Pending";

    try
    {
      // 4. Save to DB
      await _bloodUnitService.AddRequest(model);

      TempData["Success"] = "Blood request submitted successfully.";
      return RedirectToAction("MyRequests");
    }
    catch (Exception ex)
    {
      // 5. Handle errors safely
      ModelState.AddModelError("", "Error submitting request. Please try again.");
      return View("RequestUnits", model);
    }
  }
  [HttpGet]
  [Authorize(Roles = "Hospital Staff")]
  public async Task<IActionResult> MyRequests()
  {
    var hospitalId = int.Parse(User.FindFirst("HospitalID").Value);

    var data = await _bloodUnitService.GetRequestsByHospital(hospitalId);

    return View(data);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ViewRequests()
  {
    var requests = await _bloodUnitService.GetPendingRequests();
    return View(requests);
  }

  [HttpGet]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Distribute(int id)
  {
    var request = await _bloodUnitService.GetRequestById(id);

    if (request == null) return NotFound();

    var availableUnits = await _bloodUnitService.GetAvailableUnits(
        request.BloodType,
        request.RhFactor,
        request.ComponentType
    );

    // Count already assigned
    var assignedCount = await _bloodUnitService.CountAssignedUnits(id);

    var model = new DistributeViewModel
    {
      Request = request,
      AvailableUnits = availableUnits,
      AssignedCount = assignedCount,
      Remaining = request.Quantity - assignedCount
    };

    return View(model);
  }

  [HttpPost]
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Distribute(DistributeViewModel model, List<int> selectedUnits)
  {
    var request = await _bloodUnitService.GetRequestById(model.Request.RequestID);

    var assignedCount = await _bloodUnitService.CountAssignedUnits(model.Request.RequestID);
    var remaining = request?.Quantity - assignedCount;

    if (selectedUnits.Count > remaining)
    {
      ModelState.AddModelError("", "You selected more units than required.");
      return RedirectToAction("Distribute", new { id = model.Request.RequestID });
    }

    await _bloodUnitService.AssignUnitsToRequest(model.Request.RequestID, selectedUnits);

    TempData["Success"] = "Units distributed successfully.";
    return RedirectToAction("ViewRequests");
  }




}
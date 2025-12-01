using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace BDMSApp.Controllers;

[Authorize(Roles = "Donor")]
public class DonorController: Controller
{
  private readonly IDonorService _donorService;
  private readonly IBloodDriveService _bloodDriveService;

  public DonorController(IDonorService donorService, IBloodDriveService bloodDriveService)
  {
      _donorService = donorService;
      _bloodDriveService = bloodDriveService;
  }
  public async Task<IActionResult> Details()
  {
      var donors = await _donorService.GetAllDonors();
      return View(donors);
  }
    public async Task<IActionResult> DonationHistory()
  {
    var donorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(donorId) || !int.TryParse(donorId, out var donorIdInt))
    {
        return Unauthorized();
    }
    var historylist = await _donorService.GetDonationHistory(int.Parse(donorId));
    return View(historylist);
  }

  [HttpGet]
  public async Task<IActionResult> MedicalHistory()
  {
    var donorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(donorId) || !int.TryParse(donorId, out var donorIdInt))
    {
        return Unauthorized();
    }
    var historylist = await _donorService.GetMedicalHistory(int.Parse(donorId));
    return View(historylist);
  }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> MedicalHistory(List<MedicalHistoryListModel> model)
{
    if (!ModelState.IsValid)
    {
        // re-show the same view with validation errors
        return View(model);
    }

    // Save each row
    foreach (var row in model)
    {
        await _donorService.UpdateMedicalHistory(row);
        // or your Dapper / SQL update here
    }

    // redirect back to GET to avoid repost on refresh
    return RedirectToAction(nameof(MedicalHistory));
}

  [HttpGet]
  public async Task<IActionResult> ScheduleAppointment()
  {
    var drives = await _bloodDriveService.GetOpenBloodDrives(); // returns Id, Title, Location, DriveDate (at least)

    // Pass the minimal data the view needs
    ViewBag.Drives = drives
        .Select(d => new { d.BDID, d.Location, d.Title })
        .ToList();

    // Default: today’s date + current time rounded to nearest 15 min
    var now = DateTime.Now;
    var rounded = now.AddMinutes(15 - (now.Minute % 15))
                     .AddSeconds(-now.Second)
                     .AddMilliseconds(-now.Millisecond);

    var model = new ScheduleAppointmentModel
    {
        Date = DateOnly.FromDateTime(now),
        Time = TimeOnly.FromDateTime(rounded)
    };

    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> ScheduleAppointment(ScheduleAppointmentModel model)
  {
    var donorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(donorId) || !int.TryParse(donorId, out var donorIdInt))
    {
        return Unauthorized();
    }

      model.DonorID = int.Parse(donorId);

      if (!ModelState.IsValid)
      {
          var drives = await _bloodDriveService.GetOpenBloodDrives();
          ViewBag.Drives = drives.Select(d => new { d.BDID, d.Location, d.Title }).ToList();
          return View(model);
      }

      var newId = await _donorService.AddAppointment(model);
      TempData["ScheduledId"] = newId;
      return RedirectToAction("AppointmentConfirmation");
  }

  public IActionResult AppointmentConfirmation()
  {
      ViewBag.AppointmentId = TempData["ScheduledId"];
      return View();
  }
}

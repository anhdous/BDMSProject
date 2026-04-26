using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace BDMSApp.Controllers;


public class DonorController : Controller
{
    private readonly IDonorService _donorService;
    private readonly IBloodDriveService _bloodDriveService;

    public DonorController(IDonorService donorService, IBloodDriveService bloodDriveService)
    {
        _donorService = donorService;
        _bloodDriveService = bloodDriveService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DonorRecords(string search, string bloodType)
    {
        var donors = await _donorService.GetAllDonors(search, bloodType);
        ViewBag.Search = search;
        ViewBag.BloodType = bloodType;
        return View(donors);
    }
     [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(int id)
    {
        var donor = await _donorService.GetDonorById(id);
        var donations = await _donorService.GetDonationHistory(id);

        var vm = new DonorDetailsVM
        {
            Donor = donor,
            Donations = donations
        };

        return View(vm);
    }

    [Authorize(Roles = "Donor")]
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
    [Authorize(Roles = "Donor")]
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
            .Select(d => new { d.BDID, d.Location, d.Title, d.StartDate, d.EndDate })
            .ToList();

        return View(new ScheduleAppointmentModel()
        {
            Date = null,
            Time = null
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> ScheduleAppointment(ScheduleAppointmentModel model)
    {
        var donorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(donorId) || !int.TryParse(donorId, out var donorIdInt))
        {
            return Unauthorized();
        }

        model.DonorID = int.Parse(donorId);
        if (model.Time < new TimeOnly(8, 0) || model.Time > new TimeOnly(17, 30))
        {
            ModelState.AddModelError("Time", "Time must be between 8:00 AM and 5:30 PM.");
        }

        if (!ModelState.IsValid)
        {
            var drives = await _bloodDriveService.GetOpenBloodDrives();
            ViewBag.Drives = drives.Select(d => new { d.BDID, d.Location, d.Title, d.StartDate, d.EndDate }).ToList();
            return View(model);
        }

        var newId = await _donorService.AddAppointment(model);
        TempData["Success"] = "Your appointment scheduled successfully!";
        return RedirectToAction("ViewAppointments");
    }

    [HttpGet]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> ViewAppointments()
    {
        var donorIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(donorIdClaim) || !int.TryParse(donorIdClaim, out var donorIdInt))
        {
            return Unauthorized();
        }

        var appointments = await _donorService.GetAppointmentsForDonor(donorIdInt);
        return View(appointments); // e.g. List<AppointmentListModel>
    }
    [HttpGet]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> EditAppointment(int Id)
    {
        var drives = await _bloodDriveService.GetOpenBloodDrives();

        ViewBag.Drives = drives
            .Select(d => new { d.BDID, d.Location, d.Title, d.StartDate, d.EndDate })
            .ToList();

        var appointment = await _donorService.GetAppointmentsById(Id);

        if (appointment == null)
            return NotFound();

        var model = new ScheduleAppointmentModel
        {
            AppointmentID = Id,
            BDID = appointment.BDID ?? 0,
            Date = DateOnly.FromDateTime(appointment.DateTime.Date),
            Time = TimeOnly.FromDateTime(appointment.DateTime)
        };

        return View("ScheduleAppointment", model); // reuse same view
    }

    [HttpPost]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> EditAppointment(ScheduleAppointmentModel model)
    {
        await _donorService.UpdateAppointment(model);
        return RedirectToAction("ViewAppointments");
    }
    [HttpPost]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        await _donorService.DeleteAppointment(id);
        return RedirectToAction("ViewAppointments");
    }

}

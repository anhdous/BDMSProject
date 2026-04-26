using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace BDMSApp.Controllers;

public class BloodDriveController : Controller
{
    private readonly IBloodDriveService _bloodDriveService;

    public BloodDriveController(IBloodDriveService bloodDriveService)
    {
        _bloodDriveService = bloodDriveService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var drives = await _bloodDriveService.GetAllBloodDrives();
        return View(drives);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new BloodDriveModel());
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(BloodDriveModel model)
    {
        if (model.StartDate < DateTime.Now.Date)
        {
            ModelState.AddModelError("StartDate", "Start date must be today or a future date.");
        }
        if (model.EndDate < model.StartDate)
        {
            ModelState.AddModelError("EndDate", "End date must be after start date.");
        }
        if (!ModelState.IsValid)
            return View(model);

        await _bloodDriveService.AddBloodDrive(model);
        TempData["Success"] = "Blood drive created successfully!";

        return RedirectToAction("Manage");
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var drive = await _bloodDriveService.GetBloodDriveById(id);

        if (drive == null)
            return NotFound();

        return View("Create", drive); // reuse Create view
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(BloodDriveModel model)
    {
        if (!ModelState.IsValid)
            return View("Create", model);

        await _bloodDriveService.UpdateBloodDrive(model);
        TempData["Success"] = "Blood drive edited successfully!";

        return RedirectToAction("Manage");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int Id)
    {
        await _bloodDriveService.DeleteBloodDrive(Id);
        TempData["Success"] = "Blood drive deleted successfully!";
        return RedirectToAction("Manage");
    }

}

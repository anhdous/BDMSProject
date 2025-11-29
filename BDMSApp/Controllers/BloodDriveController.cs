using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace BDMSApp.Controllers;

public class BloodDriveController: Controller
{
    private readonly IBloodDriveService _bloodDriveService;

    public BloodDriveController(IBloodDriveService bloodDriveService)
    {
        _bloodDriveService = bloodDriveService;
    }
  public async Task<IActionResult> Details()
    {
        var bds = await _bloodDriveService.GetAllBloodDrives();
        return View(bds);
    }

}

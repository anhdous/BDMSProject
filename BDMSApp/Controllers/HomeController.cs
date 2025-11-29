using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BDMSApp.Models;
using ApplicationCore.Interfaces.Services;
using System.Threading.Tasks;

namespace BDMSApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IBloodDriveService bloodDriveService)
    {
        _logger = logger;
        _bloodDriveService = bloodDriveService;
    }

    private readonly IBloodDriveService _bloodDriveService;

    public async Task<IActionResult> Index()
    {
        var bds = await _bloodDriveService.GetAllBloodDrives();
        return View(bds);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

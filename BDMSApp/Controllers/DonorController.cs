using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace BDMSApp.Controllers;

public class DonorController: Controller
{
    private readonly IDonorService _donorService;

    public DonorController(IDonorService donorService)
    {
        _donorService = donorService;
    }
  public async Task<IActionResult> Details()
    {
        var donors = await _donorService.GetAllDonors();
        return View(donors);
    }
      public async Task<IActionResult> GetMedicalHistory()
    {
      var donorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
      if (string.IsNullOrEmpty(donorId) || !int.TryParse(donorId, out var donorIdInt))
      {
          return Unauthorized();
      }
      var historylist = await _donorService.GetMedicalHistory(int.Parse(donorId));
      return View(historylist);
    }

}

using ApplicationCore.Interfaces.Services;
using Infrastructure.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Data.SqlClient;
using Dapper;
namespace BDMSApp.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _cfg;

    public UserController(IUserService userService, IConfiguration cfg)
    {
        _userService = userService;
        _cfg = cfg;
    }
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginModel model)
    {
        if (!ModelState.IsValid)
            return View();

        var userSuccess = await _userService.ValidateUser(model);

        if (userSuccess != null && userSuccess.UserID > 0)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userSuccess.UserID.ToString()),
            new Claim(ClaimTypes.Email, userSuccess.Email),
            new Claim(ClaimTypes.Name, userSuccess.Username),
            new Claim(ClaimTypes.Role, userSuccess.Role)
        };

            if (userSuccess.Role == "Hospital Staff" && userSuccess.StaffID != null)
            {
                claims.Add(new Claim("StaffID", userSuccess.StaffID.ToString()));

                var hospitalId = await _userService.GetHospitalIdByStaffId(userSuccess.StaffID.Value);

                if (hospitalId != null)
                {
                    claims.Add(new Claim("HospitalID", hospitalId.ToString()));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect("~/");
        }

        ModelState.AddModelError("Password", "Invalid username or password");
        return View(model);
    }
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
    // public IActionResult DebugClaims()
    // {
    //     var lines = User.Claims.Select(c => $"{c.Type} = {c.Value}");
    //     return Content(string.Join("\n", lines));
    // }
    // public IActionResult TestRole()
    // {
    //     return Content($"IsInRole(Donor) = {User.IsInRole("Donor")}");
    // }

}

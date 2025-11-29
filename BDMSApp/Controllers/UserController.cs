using ApplicationCore.Interfaces.Services;
using Infrastructure.Services;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
namespace BDMSApp.Controllers;

public class UserController: Controller
{
  private readonly IUserService _userService;

  public UserController(IUserService userService)
  {
      _userService = userService;
  }

  [HttpGet]
  public IActionResult Login()
  {
      return View();
  }

  [HttpPost]
  public async Task<IActionResult> Login(UserLoginModel model)
  {
      if (!ModelState.IsValid)
      {
          return View();
      }
      var userSuccess = await _userService.ValidateUser(model);
      if (userSuccess!= null && userSuccess.UserID > 0)
      {
          // password matches
          // redirect to home page
          // after successful authentication
          // create a cookie, cookies are always sent from browser automatically to server
          // Inside the cookie we store encrypted information (User claims) that Server can recognize and tell 
          // whether user is logged in or not
          // Cookie should have an expiration time
          // 2 hours
          // create claims username, email, role
          var claims = new List<Claim>
          {
            new Claim(ClaimTypes.NameIdentifier, userSuccess.UserID.ToString()),
            new Claim(ClaimTypes.Email, userSuccess.Email),
            new Claim(ClaimTypes.Name, userSuccess.Username),
            new Claim(ClaimTypes.Role, userSuccess.Role)
          };
          // create cookie and cookie will have the above claims information
          // along with expiration time, don't store above information in the cookie as plain text, instead encrypt the above information
          
          // We need to tell ASP.NET application that we are using Cookie based Authentication so that
          // ASp.NET can generate Cookie based on the settings we provide
          
          //identity object that will identify the user with claims
          var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
          
          // principal object which is used by ASP.NET to recognize the USER
          // create cookie with above information
          
          //HttpContext => Encapsulates your Http Request information
          await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity));
          
          return LocalRedirect("~/");
      }
      return View(model);
  }
      public async Task<IActionResult> Logout()
  {
      await HttpContext.SignOutAsync();
      return RedirectToAction("Login");
  }

}

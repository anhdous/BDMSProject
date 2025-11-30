using System.Security.Cryptography;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;

namespace Infrastructure.Services;

public class UserService : IUserService
  {
  private readonly IUserRepository _userRepository;

  public UserService(IUserRepository userRepository)
  {
      _userRepository = userRepository;
  }
  public async Task<UserLoginSuccessModel> ValidateUser(UserLoginModel model)
  {
    var user = await _userRepository.GetUserByEmail(model.Email);
    if (user == null)
    {
        throw new Exception("Email does not exists");
    }

    if (user.Password == model.Password)
    {
      return user;
    }

    return null;
  }

}
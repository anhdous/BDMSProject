using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Services;

public interface IUserService
{
    Task<UserLoginSuccessModel> ValidateUser(UserLoginModel model);
}


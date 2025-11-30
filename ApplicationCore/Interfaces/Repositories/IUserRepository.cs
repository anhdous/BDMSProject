using ApplicationCore.Models;

namespace ApplicationCore.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserLoginSuccessModel> GetUserByEmail(string email);
}
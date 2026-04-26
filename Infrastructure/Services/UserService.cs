using System.Security.Cryptography;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using ApplicationCore.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class UserService : IUserService
{
  private readonly IUserRepository _userRepository;
  private readonly IConfiguration _cfg;

  public UserService(IUserRepository userRepository, IConfiguration cfg)
  {
    _userRepository = userRepository;
    _cfg = cfg;
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
  public async Task<int?> GetHospitalIdByStaffId(int staffId)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);

    const string sql = @"
        SELECT HospitalID
        FROM dbo.HospitalStaff
        WHERE StaffID = @StaffID;";

    return await conn.ExecuteScalarAsync<int?>(sql, new { StaffID = staffId });
  }


}
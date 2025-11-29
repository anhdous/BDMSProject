using ApplicationCore.Interfaces.Repositories;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ApplicationCore.Models;
namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
  private readonly IConfiguration _cfg;
  public UserRepository(IConfiguration cfg) {
    _cfg = cfg;
    }
  public async Task<UserLoginSuccessModel> GetUserByEmail(string email)
  {
    var cs = _cfg.GetConnectionString("Default");
    using var conn = new SqlConnection(cs);
    var user = await conn.QueryAsync<UserLoginSuccessModel>(
        "SELECT * FROM dbo.UserAccount Where Email = @Email", new { Email = email });
    return user.FirstOrDefault();
    }
}
using ProductApi.Models;
using ProductApi.Models.Dto;
namespace ProductApi.Services;

public class UserService
{
  private readonly ApplicationDbContext _dbContext;

  public UserService(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public User Authenticate(LoginDto loginDto)
  {
    var user = _dbContext.Users.SingleOrDefault(u => u.Username == loginDto.Username);

    // Check if user exists and password is correct
    if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
    {
      return null;
    }

    // Authentication successful
    return user;
  }

  public async Task<User> SaveRefreshToken(int userId, string refreshToken)
  {
    var user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);

    if (user == null)
    {
      return null;
    }

    user.RefreshToken = refreshToken;
    await _dbContext.SaveChangesAsync();

    return user;
  }

  public User GetUserByRefreshToken(string refreshToken)
  {
    var user = _dbContext.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

    if (user == null)
    {
      return null;
    }

    return user;
  }

  public async Task<User> Register(RegisterDto registerDto)
  {
    // Check if username already exists
    if (_dbContext.Users.Any(u => u.Username == registerDto.Username))
    {
      throw new ApplicationException("Username already exists");
    }

    // Create new user object
    var user = new User
    {
      Username = registerDto.Username,
      FirstName = registerDto.FirstName,
      LastName = registerDto.LastName,
      Email = registerDto.Email,
      Role = registerDto.Role
    };

    // Hash password
    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

    // Save user to database
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    return user;
  }
}

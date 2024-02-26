using System.Security.Cryptography;
using System.Text;
using DR.Database.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DR.ManageApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase {
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context) {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(string username, string password) {
        var hmac = new HMACSHA512();

        var user = new User {
            Name = username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            PasswordSalt = hmac.Key,

        };

        _context.appUsers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}

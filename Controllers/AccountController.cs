using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;
// url: http/api/account
public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{

    [HttpPost("login")] //account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) //login by email & password
    {
        var user = await context.Users.SingleOrDefaultAsync(x =>
        x.Email == loginDto.Email);         //FirstOrDefaultAsync-> return User if true, else null
                                            // tìm user theo tên sau đó mới xét tới password

        if (user == null) return Unauthorized("Invalid email or password"); // néu sai ten -> obj user = null

        using var hmac = new HMACSHA512(user.PasswordSalt);            //tim dung cthuc da ma khoa ->  

        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));//tim duoc cthuc, thi ma khoa again rồi ss , ss password de sosanh chuoi password trong db

        for (int i = 0; i < ComputeHash.Length; i++)
        {
            if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password"); // nếu 1 ký tự nào khác
        }
        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };

        return userDto;
    }


    [HttpPost("register")] // account/register

    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await EmailExists(registerDto.Email)) return BadRequest("Email has been registered");
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
        if (this.validPassword(registerDto.Password)) return BadRequest("Password length must be greater than or equal to 12");


        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            Email = registerDto.Email,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
        return userDto;
    }

    private async Task<bool> UserExists(string UserName)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == UserName.ToLower());
    }
     private async Task<bool> EmailExists(string Email)
    {
        return await context.Users.AnyAsync(x => x.Email.ToLower() == Email.ToLower());
    }
    
    private bool validPassword(string password)
    {
        return password.Length < 4;
    }
}
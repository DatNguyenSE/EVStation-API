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
}
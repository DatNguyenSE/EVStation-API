using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class Seed
{
    public static async Task SeedUsers(AppDbContext context)
    {
        if (await context.Users.AnyAsync()) return;
        
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var users = JsonSerializer.Deserialize<List<SeedUserDto>>(userData);

        if (users == null)
        {
            Console.WriteLine("No users in seed data");
            return;
        }
        using var hmac = new HMACSHA512();

        foreach (var user in users)
        {
            var appUser  = new AppUser
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("tuilanhom2")),  // same password
                PasswordSalt = hmac.Key, // same key
            };
            context.Users.Add(appUser );
        }
        await context.SaveChangesAsync(); 
    }
}

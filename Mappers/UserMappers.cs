using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Account;
using API.Entities;

namespace API.Mappers
{
    public static class UserMappers
    {
        public static StaffDto ToStaffDto(this AppUser appUser)
        {
            return new StaffDto
            {
                Id = appUser.Id,
                UserName = appUser.UserName ?? string.Empty,
                FullName = appUser.FullName,
                Email = appUser.Email ?? string.Empty
            };
        }

        public static UserDto ToUserDto(this AppUser appUser, string? roleName)
        {
            return new UserDto
            {
                Id = appUser.Id,
                FullName = appUser.FullName,
                Email = appUser.Email ?? string.Empty,
                DateOfBirth = appUser.DateOfBirth,
                EmailConfirmed = appUser.EmailConfirmed,
                Roles = roleName
            };
        }
    }
}
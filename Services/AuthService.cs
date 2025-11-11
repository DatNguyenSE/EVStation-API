using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Account;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Interfaces.IServices;
using API.Mappers;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;
        public AuthService(IUnitOfWork uow, UserManager<AppUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        public async Task<UserDto> RegisterAndSyncGuestHistoryAsync(RegisterAndSyncGuestDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email đã được sử dụng. Vui lòng đăng nhập hoặc sử dụng email khác.");
            }

            var newUser = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                DateOfBirth = registerDto.DateOfBirth
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Đăng ký thất bại: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddToRoleAsync(newUser, AppConstant.Roles.Driver);

            var userRoles = await _userManager.GetRolesAsync(newUser);
            var mainRole = userRoles.FirstOrDefault();

            var guestVehicle = await _uow.Vehicles
                .GetByPlateAsync(registerDto.GuestVehicleLicensePlate);

            if (guestVehicle != null)
            {
                guestVehicle.OwnerId = newUser.Id;
                guestVehicle.Owner = newUser;

                // var receipts = await _uow.Receipts.GetReceiptsByPlateAsync(registerDto.GuestVehicleLicensePlate);
                // foreach (var receipt in receipts)
                // {
                //     receipt.AppUserId = newUser.Id;
                //     receipt.AppUser = newUser; 
                // }
            }

            await _uow.Complete();

            return newUser.ToUserDto(mainRole);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Account;

namespace API.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAndSyncGuestHistoryAsync(RegisterAndSyncGuestDto registerDto);
    }
}
using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Interfaces
{
    public interface IResetTokenService
    {
        Task<ResetTokenEntity> GetResetByTokenAsync(string guid);
        Task<ResetTokenEntity> AddResetTokenAsync(ResetTokenEntity token);
        Task<ResetTokenEntity> GetResetByEmailAsync(string email);
        Task<bool> RemoveTokenByIdAsync(string id);
    }
}

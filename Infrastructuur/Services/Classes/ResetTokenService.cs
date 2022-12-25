using Infrastructuur.Database;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Classes
{
    public class ResetTokenService : IResetTokenService
    {
        private readonly StoryZonDbContext _storyZonDbContext;

        public ResetTokenService(StoryZonDbContext storyZonDbContext)
        {
            _storyZonDbContext = storyZonDbContext;
        }

        public async Task<ResetTokenEntity> AddResetTokenAsync(ResetTokenEntity token)
        {
            
            await _storyZonDbContext.CreateAsync<ResetTokenEntity>(token, "resetToken", new MongoDB.Bson.BsonDocument
            {
                {"token", token.Token },
                { "email", token.Email },
                 { "expirationDate" , token.ExpirationDate }
              });

            return (await _storyZonDbContext?.GetAllAsync<ResetTokenEntity>("resetToken")).FirstOrDefault(x => x.Email == token.Email);
        }

        public async Task<ResetTokenEntity> GetResetByTokenAsync(string guid)
        {
            return (await _storyZonDbContext.GetAllAsync<ResetTokenEntity>("resetToken")).LastOrDefault(x => x.Token == guid);
        }
        public async Task<ResetTokenEntity> GetResetByEmailAsync(string email)
        {
            return (await _storyZonDbContext.GetAllAsync<ResetTokenEntity>("resetToken")).LastOrDefault(x => x.Email == email);
        }

        public async Task<bool> RemoveTokenByIdAsync(string id)
        {

           if (!(await _storyZonDbContext.DeleteAsync<ResetTokenEntity>(id, "resetToken", x => x.Id == id)))
             {
                return false;
            }

            return true;
        }
    }
}

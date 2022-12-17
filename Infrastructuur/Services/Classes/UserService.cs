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
    public class UserService : IUserService
    {
        private readonly StoryZonDbContext _storyZonDbContext;

        public UserService(StoryZonDbContext storyZonDbContext)
        {
            _storyZonDbContext = storyZonDbContext;
        }

        public Task<StoryzonEntity> AddStoryToUserByUserIdAsync(string userId, string storyzonId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserEntity> AddUserAsync(UserEntity user)
        {
            if ((await _storyZonDbContext.GetAllAsync<UserEntity>("user"))
                .Any(x => x.UserName == user.UserName ||
                          (x.FirstName == user.FirstName &&
                          x.LastName == user.LastName) ||
                          x.Email == user.Email))
                {
                return user;
            }
            return await _storyZonDbContext.CreateAsync<UserEntity>(user, "user", new MongoDB.Bson.BsonDocument
            {
                { "userName", user.UserName},
                { "firstName", user.FirstName},
                { "lastName", user.LastName},
                { "password", user.Password},
                { "email", user.Email}
            });
        }

        public Task<bool> DeleteUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<StoryzonEntity>> GetAllStoriesByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<UserEntity> GetUserByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserEntity>> GetUsersAsync()
        {
            return (await _storyZonDbContext.GetAllAsync<UserEntity>("user")).ToList();
        }

        public Task<bool> RemoveStoryFromUserAsync(string userId, string storyzonId)
        {
            throw new NotImplementedException();
        }

        public Task<UserEntity> UpdateUserByIdAsync(string id, UserEntity user)
        {
            throw new NotImplementedException();
        }
    }
}

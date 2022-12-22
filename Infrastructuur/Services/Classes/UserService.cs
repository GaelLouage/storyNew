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

        public async Task<bool> AddUserAsync(UserEntity user)
        {
            var users = (await _storyZonDbContext.GetAllAsync<UserEntity>("user"));

            if (users.Any(x => x.UserName == user.UserName ||
                          (x.FirstName == user.FirstName &&
                          x.LastName == user.LastName) ||
                          x.Email == user.Email))
                {
                return false;
            }
            await _storyZonDbContext.CreateAsync<UserEntity>(user, "user", new MongoDB.Bson.BsonDocument
            {
                { "userName", user.UserName},
                { "firstName", user.FirstName},
                { "lastName", user.LastName},
                { "password", user.Password},
                { "email", user.Email},
                {"role", "User" }
            });
            return (await _storyZonDbContext.GetAllAsync<UserEntity>("user")).Count() > users.Count(); 
        }

        public async Task<bool> DeleteUserByIdAsync(string id)
        {
            var previousUserCount = (await GetUsersAsync()).Count;
            await _storyZonDbContext.DeleteAsync<UserEntity>(id,"user", x => x.Id == id);
            return (await GetUsersAsync()).Count > previousUserCount;
        }

        public Task<List<StoryzonEntity>> GetAllStoriesByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<UserEntity> GetUserByIdAsync(string id)
        {
            return (await GetUsersAsync()).FirstOrDefault(x => x.Id == id);
        }

        public async Task<UserEntity> GetUserByNameAsync(string userName)
        {
            return (await _storyZonDbContext.GetAllAsync<UserEntity>("user"))
                .FirstOrDefault(x => x.UserName == userName);
                
        }

        public async Task<List<UserEntity>> GetUsersAsync()
        {
            return (await _storyZonDbContext.GetAllAsync<UserEntity>("user")).ToList();
        }

        public Task<bool> RemoveStoryFromUserAsync(string userId, string storyzonId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserByIdAsync(string id, UserEntity user)
        {
            if(!(await _storyZonDbContext.UpdateAsync<UserEntity>(id, new Dictionary<string, string>()
            {
                {"userName", user.UserName },
                {"firstName",user.FirstName},
                {"lastName", user.LastName},
                {"passWord", user.Password },
                {"email", user.Email },
                {"role", user.Role }
            }, "user")))
            {
                return false;
            }
            return  true;
        }
    }
}

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

        public Task<UserEntity> AddUserAsync(StoryzonEntity storyzon)
        {
            throw new NotImplementedException();
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

        public Task<List<UserEntity>> GetUsersAsync()
        {
            throw new NotImplementedException();
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

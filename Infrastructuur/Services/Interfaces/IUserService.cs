using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserEntity>> GetUsersAsync();
        Task<UserEntity> GetUserByIdAsync(string id);
        Task<bool> AddUserAsync(UserEntity user);
        Task<bool> DeleteUserByIdAsync(string id);
        Task<bool> UpdateUserByIdAsync(string id, UserEntity user);
        // favorite stories 
        Task<List<StoryzonEntity>> GetAllStoriesByUserIdAsync(string userId);
        Task<StoryzonEntity> AddStoryToUserByUserIdAsync(string userId, string storyzonId);
        Task<bool> RemoveStoryFromUserAsync
            (string userId, string storyzonId);

        Task<UserEntity> GetUserByNameAsync(string userName);    

    }
}

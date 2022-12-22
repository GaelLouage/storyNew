using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Interfaces
{
    public  interface IUserSelectedStoryService
    {
        Task<IQueryable<StoryzonEntity>> GetStoryzonsByUserSelectedIdAsync(string userId);
        Task<bool> AddSelectedStoryToUserByIdAsync(UserStorySelectEntity addy);
    }
}

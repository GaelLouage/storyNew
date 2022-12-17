using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Interfaces
{
    public interface IStoryZonService
    {
        Task<List<StoryzonEntity>> GetStoryzonsAsync();
        Task<StoryzonEntity> GetStoryzonByIdAsync(string id);
        Task<StoryzonEntity> AddStoryZonAsync(StoryzonEntity storyzon);
        Task<bool> DeleteStoryzonByIdAsync(string id);
        Task<bool> UpdateStoryZonByIdAsync(string id, StoryzonEntity storyzon);
    }
}

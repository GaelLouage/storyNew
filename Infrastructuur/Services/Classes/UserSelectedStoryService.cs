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
    public class UserSelectedStoryService : IUserSelectedStoryService
    {
        private readonly StoryZonDbContext _storyZonDbContext;

        public UserSelectedStoryService(StoryZonDbContext storyZonDbContext)
        {
            _storyZonDbContext = storyZonDbContext;
        }

        public async Task<bool> AddSelectedStoryToUserByIdAsync(UserStorySelectEntity addy)
        {

            var currentCount = (await _storyZonDbContext.GetAllAsync<UserStorySelectEntity>("userStorySelected")).ToList().Count;
            await _storyZonDbContext.CreateAsync<UserStorySelectEntity>(addy, "userStorySelected", new MongoDB.Bson.BsonDocument
            {
                {"userId", addy.UserId},
                {"storyId",addy.StoryId}
            });

            return (await _storyZonDbContext.GetAllAsync<UserStorySelectEntity>("userStorySelected")).ToList().Count > currentCount;
        }

        public async Task<IQueryable<StoryzonEntity>> GetStoryzonsByUserSelectedIdAsync(string userId)
        {
            var allStories = await _storyZonDbContext.GetAllAsync<StoryzonEntity>("storyzon");
            var selectedStoriesIds = (await _storyZonDbContext.GetAllAsync<UserStorySelectEntity>("userStorySelected"))
                 .Where(x => x.UserId == userId).Select(x => x.StoryId).ToList();
          
            var newStories = new List<StoryzonEntity>();
            foreach (var storyID in selectedStoriesIds)
            {
                foreach (var story in allStories)
                {
                    if (story.Id == storyID)
                    {
                        newStories.Add(story);
                    }
                }
            }
            return newStories.AsQueryable();
        }
    }
}

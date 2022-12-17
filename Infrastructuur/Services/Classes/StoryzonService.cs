using Infrastructuur.Database;
using Infrastructuur.Models;
using Infrastructuur.Services.Interfaces;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Classes
{
    public class StoryzonService : IStoryZonService
    {
        private readonly StoryZonDbContext _storyZonDbContext;

        public StoryzonService(StoryZonDbContext storyZonDbContext)
        {
            _storyZonDbContext = storyZonDbContext;
        }

        public async Task<StoryzonEntity> AddStoryZonAsync(StoryzonEntity storyzon)
        {
            return await _storyZonDbContext.CreateAsync<StoryzonEntity>(storyzon, "storyzon",
             new MongoDB.Bson.BsonDocument
            {
                { "title", storyzon.Title },
                { "bodyFr", storyzon.BodyFr ?? "" },
                { "bodyNl", storyzon.BodyNl ?? "" },
                { "bodyEn", storyzon.BodyEn ?? ""},
                { "genre", storyzon.Genre },
                { "image", storyzon.Image },
                {"addedDate", storyzon.AddedDate },
                { "rating", storyzon.rating.ToString() ?? "0" }
            }); 
        }

        public async Task<bool> DeleteStoryzonByIdAsync(string id)
        {
            if(!await _storyZonDbContext.DeleteAsync<StoryzonEntity>(id, "storyzon", x => x.Id == id))
            {
                return false;
            }
            return true;
        }

        public async Task<StoryzonEntity> GetStoryzonByIdAsync(string id)
        {
            var story = await _storyZonDbContext.GetByIdAsync<StoryzonEntity>(x => x.Id == id, "storyzon");
            return story;
        }

        public async Task<List<StoryzonEntity>> GetStoryzonsAsync()
        {
            var stories = await _storyZonDbContext.GetAllAsync<StoryzonEntity>("storyzon");
            return stories.ToList();
        }

        public async Task<bool> UpdateStoryZonByIdAsync(string id, StoryzonEntity storyzon)
        {
            var fieldsToUpdate = new Dictionary<string, string?>()
            {
                { "title", storyzon.Title },
                { "bodyFr", storyzon.BodyFr ?? "" },
                { "bodyNl", storyzon.BodyNl ?? "" },
                { "bodyEn", storyzon.BodyEn ?? ""},
                { "image", storyzon.Image ?? ""},
                { "genre", storyzon.Genre ?? "" },
                { "rating", storyzon.rating.ToString() ?? "0" }
            };
            if(!await _storyZonDbContext.UpdateAsync<StoryzonEntity>(id, fieldsToUpdate, "storyzon"))
            {
                return false;
            }
            return true;
        }
    }
}

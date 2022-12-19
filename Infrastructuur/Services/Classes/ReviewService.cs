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
    public class ReviewService : IReviewService
    {
        private readonly StoryZonDbContext _storyZonDbContext;

        public ReviewService(StoryZonDbContext storyZonDbContext)
        {
            _storyZonDbContext = storyZonDbContext;
        }

        public async Task<ReviewEntity> AddReview(ReviewEntity review)
        {
            // unable to add another review if you already added one with a certain user
            if((await GetReviews())
                .Any(x => x.UserId == review.UserId && x.StoryId == review.StoryId))
            {
                return null;
            }
        
            return await _storyZonDbContext.CreateAsync<ReviewEntity>(review, "review", new BsonDocument
            {
                {"reviewTitle", review.ReviewTitle },
                {"reviewBody", review.ReviewBody},
                {"userId", new ObjectId(review.UserId) },
                {"storyId", new ObjectId(review.StoryId) },
                {"addedDate", DateTime.Now.ToString("dd/MM/yyyy") },
                {"rating", review.Rating ?? "0" }
            });
        }

        public Task<ReviewEntity> GetReviewById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ReviewEntity>> GetReviews() =>
            (await _storyZonDbContext.GetAllAsync<ReviewEntity>("review")).ToList();
        

        public async Task<List<ReviewEntity>> GetReviewsByStoryId(string storyId) =>
            (await _storyZonDbContext.GetAllAsync<ReviewEntity>("review")).Where(x => x.StoryId == storyId).ToList();

        

        public async Task<List<ReviewEntity>> GetReviewsByUserId(string userId) =>
            (await _storyZonDbContext.GetAllAsync<ReviewEntity>("review")).Where(x => x.UserId == userId).ToList();

        public async Task<bool> RemoveReview(string id)
        {
            var currentReviewCount = (await GetReviews()).Count;

            await _storyZonDbContext.DeleteAsync<ReviewEntity>(id, "review", x => x.ReviewId == id);
            //return true if deleted
            return (await GetReviews()).Count < currentReviewCount;
        }
    }
}

using Infrastructuur.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewEntity>> GetReviews();
        Task<List<ReviewEntity>> GetReviewsByUserId(string userId);
        Task<List<ReviewEntity>> GetReviewsByStoryId(string storyId);
        Task<ReviewEntity> GetReviewById(string id);    
        Task<ReviewEntity> AddReview(ReviewEntity entity);
        Task<bool> RemoveReview(string reviewId);
    }
}

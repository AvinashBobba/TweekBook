using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweekBook.Domain;

namespace TweekBook.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);

        Task<bool> CreatePostAsync(Post post);

        Task<bool> UserOwnsPostAsync(Guid postId, string userId);

        Task<List<Tags>> GetAllTagsAsync();
        Task<bool> CreateTagAsync(Tags newTag);
    }
}

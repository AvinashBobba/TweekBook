using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TweekBook.Contracts.V1.Requests;
using TweekBook.Contracts.V1.Responses;

namespace TweekBook.SDK
{
    [Headers("Authorization: Bearer")]
    public interface ITweetBookApi
    {
        [Get(path: "/api/v1/posts")]
        Task<Response<List<PostResponse>>> GetaAllAsync();

        [Get(path: "/api/v1/posts/{postId}")]
        Task<Response<List<PostResponse>>> GetAsync(Guid postId);
        
        [Post(path: "/api/v1/posts")]
        Task<Response<List<PostResponse>>> CreateAsync([Body] CreatePostRequest createPostRequest);

        [Put(path: "/api/v1/posts/{postId}")]
        Task<Response<List<PostResponse>>> UpdateAsync([Body] UpdatePostRequest updatePostRequest);

        [Put(path: "/api/v1/posts/{postId}")]
        Task<Response<List<PostResponse>>> DeleteAsync(Guid postId);
    }
}

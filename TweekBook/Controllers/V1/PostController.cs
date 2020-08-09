using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using TweekBook.Cache;
using TweekBook.Contracts.V1;
using TweekBook.Contracts.V1.Requests;
using TweekBook.Contracts.V1.Responses;
using TweekBook.Domain;
using TweekBook.Extensions;
using TweekBook.Services;

namespace TweekBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
     public class PostController : Controller
    {
        private readonly IPostService _postService;

        private readonly IMapper _mapper;

        public PostController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        [Cached(timeToLiveSeconds:600)]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _postService.GetPostsAsync();
            var postResponses = _mapper.Map<List<PostResponse>>(posts);
            return Ok(postResponses);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest createPostRequest)
        {
            var newPostId = Guid.NewGuid();
            var post = new Post
            {
                Id = newPostId,
                Name = createPostRequest.Name,
                UserId = HttpContext.GetUserId(),
                Tags = createPostRequest.Tags.Select(x => new PostTag { PostId = newPostId, TagName = x }).ToList()
            };

            await _postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = _mapper.Map<Post>(post);
            return Created(locationUri, response);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId ,[FromBody] UpdatePostRequest updatePostRequest)
        {
            var userOwnsPost = await  _postService.UserOwnsPostAsync(postId,HttpContext.GetUserId());

            if(!userOwnsPost)
            {
                return BadRequest(new { error = "You don't own this post"}); 
            }

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = updatePostRequest.Name;

            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
                return Ok(_mapper.Map<Post>(post));

            return NotFound();
        }


        [HttpGet(ApiRoutes.Posts.Get)]
        [Cached(timeToLiveSeconds: 600)]
        public async Task<IActionResult> GetById([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();

            return Ok(_mapper.Map<Post>(post));
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You don't own this post" });
            }

            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted)
                return NoContent();
            return NotFound();
        }
    }
}

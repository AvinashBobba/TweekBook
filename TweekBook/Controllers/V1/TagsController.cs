using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TweekBook.Contracts.V1;
using TweekBook.Contracts.V1.Requests;
using TweekBook.Contracts.V1.Responses;
using TweekBook.Domain;
using TweekBook.Extensions;
using TweekBook.Services;

namespace TweekBook.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces(contentType: "application/json")]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        public TagsController(IPostService postService,IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all the tags 
        /// </summary>
        /// <response code="200">Returns all the tags</response>
        [HttpGet(ApiRoutes.Tags.GetAll)]
        [Authorize(Policy = "MustWorkForABMakers")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _postService.GetAllTagsAsync();
            var tagResponse = _mapper.Map<List<TagResponse>>(tags);
            return Ok(tagResponse);
        }

        /// <summary>
        /// Create A tag
        /// </summary>
        /// <param name="request"></param>
        /// <returns code="201">Created tag</returns>
        /// <response code="400">Unable to create tags due to validation erros</response>
        [HttpPost(ApiRoutes.Tags.Create)]
        [ProducesResponseType(typeof(TagResponse),statusCode: 201)]
        [ProducesResponseType(typeof(ErrorResponse),statusCode: 400)]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            var newTag = new Tags
            {
                Name = request.TagName,
                CreatorId = HttpContext.GetUserId(),
                CreatedOn = DateTime.UtcNow
            };

            var created = await _postService.CreateTagAsync(newTag);
            if (!created)
            {
                return BadRequest(new ErrorResponse{ Errors = new List<ErrorModel> { new ErrorModel { Message = "Unable to create tag" } } });
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Tags.Get.Replace("{tagName}", newTag.Name);
            return Created(locationUri, _mapper.Map<TagResponse>(newTag));
        }
    }
}

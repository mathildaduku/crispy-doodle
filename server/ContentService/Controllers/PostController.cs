using AutoMapper;
using ContentService.Dto.Request;
using ContentService.Helpers;
using ContentService.Models;
using ContentService.Services.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ContentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUserService _userService;
        private readonly IPostService _postService;

        public PostController(IMapper mapper, IPublishEndpoint publishEndpoint, IUserService userService, IPostService postService)
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _userService = userService;
            _postService = postService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            var user = _userService.GetUserById(userId);

            if (user == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            var post = _mapper.Map<Post>(createPostDto);
            post.Author = Guid.Parse(userId);
            _postService.Add(post);

            var result = await _postService.SaveChangesAsync();

            if (!result)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Something went wrong";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            await _publishEndpoint.Publish(_mapper.Map<PostCreated>(post));

            _response.Message = "Post created successfully";
            _response.Result = new { post };
            return Ok(_response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] PostSearchParams searchParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = null;

            if (userId != null)
            {
                user = await _userService.GetUserById(userId ?? "");
            }

            var payload = await _postService.GetAll(searchParams, user?.Id);

            _response.Status = ResponseStatus.Success;
            _response.Message = "Posts fetched successfully";
            _response.Result = payload;

            return Ok(payload);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetAPost(Guid postId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //User? user = null;

            //if (userId != null)
            //{
            //    user = await _userService.GetUserById(userId ?? "");
            //}

            var payload = await _postService.GetById(postId);

            _response.Status = ResponseStatus.Success;
            _response.Message = "Posts fetched successfully";
            _response.Result = payload;

            return Ok(payload);
        }
    }
}

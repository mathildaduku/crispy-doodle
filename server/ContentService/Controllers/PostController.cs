using AutoMapper;
using ContentService.Dto.Request;
using ContentService.Extensions;
using ContentService.Helpers;
using ContentService.Models;
using ContentService.Services.Interfaces;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly ILikeService _likeService;
        private readonly ILogger<CommentController> _logger;


        public PostController(IMapper mapper, IPublishEndpoint publishEndpoint, IUserService userService, IPostService postService, ILikeService likeService, ILogger<CommentController> logger)
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _userService = userService;
            _postService = postService;
            this._likeService = likeService;
            this._logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto createPostDto)
        {
            _logger.LogInformation("Creating a post {@Post}", createPostDto);
            Guid userId = User.GetUserId();

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogError($"No User Profile found: {userId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            var post = _mapper.Map<Post>(createPostDto);
            post.UserId = userId;
            _postService.Add(post);

            var result = await _postService.SaveChangesAsync();

            if (!result)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Something went wrong";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
            _logger.LogInformation("Post created successfully");

            _logger.LogInformation("Publishing post created event to the service bus");

            await _publishEndpoint.Publish(_mapper.Map<PostCreated>(post));

            _logger.LogInformation("Finished publishing post created event to the service bus");

            _response.Message = "Post created successfully";
            _response.Result = new { post };
            return Ok(_response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] PostSearchParams searchParams)
        {
            _logger.LogInformation("Fetching posts");

            Guid userId = User.GetUserId();

            _logger.LogInformation($"Fetching user by Id: {userId}");
            var user = await _userService.GetUserById(userId);

            _logger.LogInformation($"Fetching posts for user with Id: {userId}");
            var payload = await _postService.GetAll(searchParams, user?.Id);

            _logger.LogInformation($"Posts fetched successfully for user with Id: {userId}");

            _response.Status = ResponseStatus.Success;
            _response.Message = "Posts fetched successfully";
            _response.Result = payload;

            return Ok(payload);
        }

         [HttpGet("{postId:guid}")]
        public async Task<IActionResult> GetAPost(Guid postId)
        {

            Guid userId = User.GetUserId();

            _logger.LogInformation($"Fetching post with Id: {postId} for user with id: {userId}");

            var post = await _postService.GetByIdForAUser(postId, userId);

            if(post == null)
            {
                _logger.LogError($"Unable to find post with Id: {postId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Post does not exist";
                return BadRequest(_response);
            }

            _logger.LogInformation($"Fetching post with Id: {postId} for user with id: {userId} was successful");

            _response.Status = ResponseStatus.Success;
            _response.Message = "Post fetched successfully";
            _response.Result = post;

            return Ok(_response);
        }

        [HttpPost("like/{postId:guid}")]
        [Authorize]
        public async Task<IActionResult> LikeAPost(Guid postId)
        {
            Guid userId = User.GetUserId();

            _logger.LogInformation($"Liking post with Id: {postId} for user with id: {userId}");

            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogError($"No User Profile found: {userId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            _logger.LogInformation($"Searching for post with Id: {postId} for user with id: {userId}");

            var post = await _postService.GetById(postId);

            if (post == null)
            {

                _logger.LogError($"Unable to find post with Id: {postId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Post does not exist";
                return BadRequest(_response);
            }

            var like = new Like
            {
                PostId = post.Id,
                UserId = user.Id,
            };

            _likeService.Add(like);
            post.LikeCount++;

            _logger.LogInformation($"Increasing like count for post with Id: {postId} to {post.LikeCount}");

            _postService.Update(post);

            var result = await _likeService.SaveChangesAsync();

            if (!result)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to like post";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            _logger.LogInformation($"Post with Id: {postId} liked successfully");
            _response.Status = ResponseStatus.Success;
            _response.Message = "Post liked successfully";
            return Ok(_response);
        }

        [HttpPost("unlike/{postId:guid}")]
        [Authorize]
        public async Task<IActionResult> UnLikeAPost(Guid postId)
        {
           Guid userId = User.GetUserId();

            _logger.LogInformation($"Unliking post with Id: {postId} for user with id: {userId}");


            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                _logger.LogError($"No User Profile found: {userId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            _logger.LogInformation($"Searching for post with Id: {postId} for user with id: {userId}");

            var post = await _postService.GetById(postId);

            if (post == null)
            {
                _logger.LogError($"Unable to find post with Id: {postId}");

                _response.Status = ResponseStatus.Error;
                _response.Message = "Post does not exist";
                return BadRequest(_response);
            }

            var like = await _likeService.GetUserLastLikeForAPost(userId, postId);

            if(like == null)
            {
                _logger.LogError($"User with id {userId} hasn't liked post with Id: {postId}. Returning early with success response");

                _response.Status = ResponseStatus.Success;
                _response.Message = "Post unliked successfully";
                return Ok(_response);
            }

            _likeService.Remove(like);
            post.LikeCount--;

            _logger.LogInformation($"Decreasing like count for post with Id: {postId} to {post.LikeCount}");


            _postService.Update(post);

            var result = await _likeService.SaveChangesAsync();

            if (!result)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to unlike post";
                return new ObjectResult(_response) { StatusCode = 500 };
            }

            _logger.LogInformation($"Post with Id: {postId} unliked successfully");

            _response.Status = ResponseStatus.Success;
            _response.Message = "Post unliked successfully";
            return Ok(_response);
        }
    }
}

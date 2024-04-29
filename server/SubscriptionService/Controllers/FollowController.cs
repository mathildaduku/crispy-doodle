using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Helpers;
using SubscriptionService.Services.Interfaces;


namespace SubscriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();
        private readonly IUserIdentityService _userIdentityService;
        private readonly ILogger<FollowController> _logger;

        public FollowController(IFollowService followService, IMapper mapper, IUserService userService, IUserIdentityService userIdentityService, ILogger<FollowController> logger)
        {
            _followService = followService;
            _mapper = mapper;
            _userService = userService;
            _userIdentityService = userIdentityService;
            _logger = logger;
        }

        [HttpPost("follow/{followeeId:guid}")]
        [Authorize]
        public async Task<IActionResult> FollowUserAsync(Guid followeeId)
        {
            try
            {
                var followerId = _userIdentityService.GetUserIdFromClaims(User);
                if (followerId == Guid.Empty)
                {
                    _response.Message = "User not authorized";
                    return Unauthorized();
                }

                // Check if the user to be followed exists.
                var userToFollow = await _userService.GetUserById(followeeId);
                if (userToFollow == null)
                {
                    _response.Message = "User to follow not found.";
                    return NotFound(_response);
                }

                // Check if the authenticated user is already following the target user.
                var isFollowing = await _followService.IsFollowingAsync(followerId, followeeId);
                if (isFollowing)
                {
                    _response.Message = "You are already following this user.";
                    return BadRequest(_response);
                }

                // Create a new follow relationship.
                await _followService.FollowUserAsync(followerId, followeeId);
               _response.Message = "Successfully followed user.";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while following the user with the: {followeeId}.");
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpDelete("unfollow/{followeeId:guid}")]
        [Authorize]
        public async Task<IActionResult> UnfollowUserAsync(Guid followeeId)
        {
            try
            {
                var followerId = _userIdentityService.GetUserIdFromClaims(User);
                if (followerId == Guid.Empty)
                {
                    _response.Message = "User not authorized";
                    return Unauthorized();
                }

                // Check if the authenticated user is already following the target user.
                var isFollowing = await _followService.IsFollowingAsync(followerId, followeeId);
                if (!isFollowing)
                {
                    _response.Message = "You are not following this user.";
                    return BadRequest(_response);
                }

                // Remove the follow relationship.
                await _followService.UnfollowUserAsync(followerId, followeeId);

                _response.Message = "Successfully unfollowed user.";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while unfollowing the user with the id: {followeeId}.");
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("followers")]
        [Authorize]
        public async Task<IActionResult> GetFollowersAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var userId = _userIdentityService.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    _response.Message = "User not authorized";
                    return Unauthorized();
                }

                // Retrieve followers of the authenticated user.
                var followers = await _followService.GetFollowersAsync(userId, pageNumber, pageSize);
                _response.Message = "Successfully retrieved followers.";
                return Ok(followers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving followers.");
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("followees")]
        [Authorize]
        public async Task<IActionResult> GetFolloweesAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var userId = _userIdentityService.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    _response.Message = "User not authorized";
                    return Unauthorized();
                }

                // Retrieve followees of the authenticated user.
                var followees = await _followService.GetFolloweesAsync(userId, pageNumber, pageSize);
                _response.Message = "Successfully retrieved followees";
                return Ok(followees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving followees.");
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }
    }
}

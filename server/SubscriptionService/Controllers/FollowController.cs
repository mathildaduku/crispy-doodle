using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Dto.Request;
using SubscriptionService.Helpers;
using SubscriptionService.Services.Interfaces;
using System.Security.Claims;

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
        private readonly ISubService _subService;

        public FollowController(IFollowService followService, IMapper mapper, IUserService userService, ISubService subService)
        {
            _followService = followService;
            _mapper = mapper;
            _userService = userService;
            _subService = subService;
        }

        [HttpPost("follow/{followeeId:guid}")]
        [Authorize]
        public async Task<IActionResult> FollowUserAsync(Guid followeeId)
        {
            try
            {
                var followerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(followerId))
                {
                    _response.Message = "User not found.";
                    return NotFound(_response);
                }

                // Check if the user to be followed exists.
                var userToFollow = await _userService.GetUserById(followeeId);
                if (userToFollow == null)
                {
                    _response.Message = "User to follow not found.";
                    return NotFound(_response);
                }

                var followeeIdString = followeeId.ToString();

                // Check if the authenticated user is already following the target user.
                var isFollowing = await _followService.IsFollowingAsync(followerId, followeeIdString);
                if (isFollowing)
                {
                    _response.Message = "You are already following this user.";
                    return BadRequest(_response);
                }

                // Create a new follow relationship.
                await _followService.FollowUserAsync(followerId, followeeIdString);
               _response.Message = "Successfully followed user.";
                return Ok(_response);
            }
            catch (Exception ex)
            {
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
                var followerId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(followerId))
                {
                    _response.Message = "User not found.";
                    return NotFound(_response);
                }

                var followeeIdString = followeeId.ToString();

                // Check if the authenticated user is already following the target user.
                var isFollowing = await _followService.IsFollowingAsync(followerId, followeeIdString);
                if (!isFollowing)
                {
                    _response.Message = "You are not following this user.";
                    return BadRequest(_response);
                }

                // Remove the follow relationship.
                await _followService.UnfollowUserAsync(followerId, followeeIdString);

                _response.Message = "Successfully unfollowed user.";
                return Ok(_response);
            }
            catch (Exception ex)
            {
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
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userId))
                {
                    _response.Message = "User not found.";
                    return NotFound(_response);
                }

                // Retrieve followers of the authenticated user.
                var followers = await _followService.GetFollowersAsync(userId, pageNumber, pageSize);
                _response.Message = "Successfully retrieved followers.";
                return Ok(followers);
            }
            catch (Exception ex)
            {
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
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userId))
                {
                    _response.Message = "User not found.";
                    return NotFound(_response);
                }

                // Retrieve followees of the authenticated user.
                var followees = await _followService.GetFolloweesAsync(userId, pageNumber, pageSize);
                _response.Message = "Successfully retrieved followees";
                return Ok(followees);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }
    }
}

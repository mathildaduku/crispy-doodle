using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Helpers;
using SubscriptionService.Services.Interfaces;
using System.Security.Claims;

namespace SubscriptionService.Controllers
{
    [Route("api/sub")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IFollowService _followService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();

        public SubscriptionController(IFollowService followService, IMapper mapper, IUserService userService)
        {
            _followService = followService;
            _mapper = mapper;
            _userService = userService;
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
                    _response.Message = "User not found..";
                    return NotFound(_response);
                }

                // Check if the user to be followed exists.
               /* var userToFollow = await _userService.GetUserById(followeeId);
                if (userToFollow == null)
                {
                    _response.Message = "User to follow not found.";
                    return NotFound(_response);
                }*/

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

    }
}

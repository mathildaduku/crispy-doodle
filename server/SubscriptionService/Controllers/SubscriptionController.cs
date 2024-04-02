using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Data;
using SubscriptionService.Dto.Request;
using SubscriptionService.Helpers;
using SubscriptionService.Services.Interfaces;
using System.Security.Claims;

namespace SubscriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubService _subService;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();
        private readonly AppDbContext _context;
        private readonly IFollowService _followService;
        private readonly IMapper _mapper;

        public SubscriptionController( ISubService subService, AppDbContext appDbContext, IFollowService followService, IUserService userService, IMapper mapper)
        {
            _subService = subService;
            _context = appDbContext;
            _followService = followService;
            _mapper = mapper;   
        }


        [HttpPost("subscribe")]
        [Authorize]
        public async Task<IActionResult> SubscribeAsync(SubDto requestDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var response = await _subService.SubscribeAsync(userId, requestDto);
                _response.Status = ResponseStatus.Success;
                _response.Message = "Subscribed successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Something went wrong";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
        }

        [HttpDelete("unsubscribe/{subscriptionId:guid}")]
        [Authorize]
        public async Task<IActionResult> UnsubscribeAsync(Guid subscriptionId)
        {
            try
            {
                var result = await _subService.UnsubscribeAsync(subscriptionId);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "An error occurred while unsubscribing.";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
        }

        [HttpGet("subscriptions")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptions()
        {
            try
            {
                var subscriberUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(subscriberUserId))
                {
                    return Unauthorized();
                }

                var subscriptions = await _subService.GetSubscriptionsForUserAsync(subscriberUserId);
                if (subscriptions == null)
                {
                    return NotFound();
                }

                _response.Status = ResponseStatus.Success;
                _response.Message = "Fetched subscriptions successfully";
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "An error occurred while fetching subscriptions.";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
        }

        [HttpGet("issubscribed/{targetUserId}")]
        [Authorize]
        public async Task<IActionResult> IsSubscribed(string targetUserId)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var isSubscribed = await _subService.IsSubscribedAsync(userId, targetUserId);
                _response.Status = ResponseStatus.Success;
                _response.Message = "Subscription status checked successfully";
                return Ok(isSubscribed);
            }
            catch (Exception ex)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "An error occurred while checking subscription status.";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
        }

    }
}

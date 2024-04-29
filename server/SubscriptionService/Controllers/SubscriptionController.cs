using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubscriptionService.Data;
using SubscriptionService.Dto.Request;
using SubscriptionService.Helpers;
using SubscriptionService.Services.Interfaces;


namespace SubscriptionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubService _subService;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();
        private readonly IUserIdentityService _userIdentityService;
        private readonly ILogger<SubscriptionController> _logger;
        public SubscriptionController( ISubService subService, IUserIdentityService userIdentityService, ILogger<SubscriptionController> logger)
        {
            _subService = subService;
            _userIdentityService = userIdentityService;
            _logger = logger;
        }


        [HttpPost("subscribe")]
[Authorize]
public async Task<IActionResult> SubscribeAsync(SubDto requestDto)
{
    Guid userId = Guid.Empty;
    try
    {
        userId = _userIdentityService.GetUserIdFromClaims(User);
        if (userId == Guid.Empty)
        {
            _response.Message = "User not authorized";
            return Unauthorized();
        }

        var response = await _subService.SubscribeAsync(userId, requestDto);
        _response.Status = ResponseStatus.Success;
        _response.Message = "Subscribed successfully";
        return Ok(response);
    }
    catch (InvalidOperationException ex)
    {
        //exception thrown when user is not following the target user
        _response.Status = ResponseStatus.Error;
        _response.Message = ex.Message;
        _logger.LogError(ex, $"An error occurred while user with ID {userId} was subscribing to {requestDto.TargetUserId} and is not following the target user.");
        return BadRequest(_response);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"An unexpected error occurred while user with ID {userId} was subscribing to {requestDto.TargetUserId}");
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
                    _response.Message = "User not authorized";
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while unsubscribing {subscriptionId}");
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
                var subscriberUserId = _userIdentityService.GetUserIdFromClaims(User);
                if (subscriberUserId == Guid.Empty)
                {
                    _response.Message = "User not authorized";
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
                _logger.LogError(ex, "An error occured while fetching subscriptions");
                _response.Status = ResponseStatus.Error;
                _response.Message = "An error occurred while fetching subscriptions.";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
        }

        [HttpGet("issubscribed/{targetUserId}")]
        [Authorize]
        public async Task<IActionResult> IsSubscribed(Guid targetUserId)
        {
            try
            {
                var userId = _userIdentityService.GetUserIdFromClaims(User);
                if (userId == Guid.Empty)
                {
                    _response.Message = "User not authorized";
                    return Unauthorized();
                }

                var isSubscribed = await _subService.IsSubscribedAsync(userId, targetUserId);
                _response.Status = ResponseStatus.Success;
                _response.Message = "Subscription status checked successfully";
                return Ok(isSubscribed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occured while checking subscription status for the target user id {targetUserId}");
                _response.Status = ResponseStatus.Error;
                _response.Message = "An error occurred while checking subscription status.";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
        }

    }
}

using AccountService.Dto.Request;
using AccountService.Dto.Response;
using AccountService.Helpers;
using AccountService.Models;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();
        private readonly IPublishEndpoint _publishEndpoint;

        public UserController(IMapper mapper, UserManager<User> userManager, IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _userManager = userManager;
            _publishEndpoint = publishEndpoint;

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            _response.Message = "User profile gotten successfully";
            _response.Result = new { User = _mapper.Map<UserDto>(user) };

            return Ok(_response);
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            _response.Message = "User profile gotten successfully";
            _response.Result = new { User = _mapper.Map<UserDto>(user) };
            return Ok(_response);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAccountProfile(UpdateProfileDto updateProfileDto)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.Bio = updateProfileDto.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            await _publishEndpoint.Publish(_mapper.Map<AccountUpdated>(user));

            _response.Message = "User Profile updated successfully";
            _response.Result = new { User = _mapper.Map<UserDto>(user) };
            return Ok(_response);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAccountProfile()
        {

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to find profile info";
                return NotFound(_response);
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Unable to delete account";
                return new ObjectResult(_response) { StatusCode = 500 };
            }
            await _publishEndpoint.Publish(_mapper.Map<AccountDeleted>(user));


            return NoContent();
        }
    }
}

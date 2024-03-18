using AccountService.Dto.Request;
using AccountService.Dto.Response;
using AccountService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public UserController(IMapper mapper, UserManager<User> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { Message = "User Profile gotten successfully", User = _mapper.Map<UserDto>(user) });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }
            return Ok(new { Message = "User Individual profile gotten succesfully", User = _mapper.Map<UserDto>(user) });
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAccountProfile(UpdateProfileDto updateProfileDto) 
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            
            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.Bio = updateProfileDto.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok(new { Message = "User Profile updated successfully", User = _mapper.Map<UserDto>(user) });
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteAccountProfile()
        {

            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            if (userId == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}

using AccountService.Dto.Request;
using AccountService.Dto.Response;
using AccountService.Helpers;
using AccountService.Models;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfiguration _configuration;
        private readonly ApiResponse<object> _response = new ApiResponse<object>();

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _signInManager = signInManager;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);


            if (!result.Succeeded)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = result.Errors.FirstOrDefault()?.Description ?? string.Empty;
                return BadRequest(_response);
            }

            await _publishEndpoint.Publish(_mapper.Map<AccountCreated>(user));

            _response.Status = ResponseStatus.Success;
            _response.Message = "User created successfully";
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Invalid username or password";
                return BadRequest(_response);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _response.Status = ResponseStatus.Error;
                _response.Message = "Invalid username or password";
                return BadRequest(_response);
            }

            // Generate authentication token
            var token = GenerateJwtToken(user);


            _response.Status = ResponseStatus.Success;
            _response.Message = "User logged in successfully";
            _response.Result = new { Token = token, User = _mapper.Map<UserDto>(user) };
            return Ok(_response);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName)
                // Add additional claims if needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"]));

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

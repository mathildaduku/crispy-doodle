using System.ComponentModel.DataAnnotations;

namespace AccountService.Dto.Request
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

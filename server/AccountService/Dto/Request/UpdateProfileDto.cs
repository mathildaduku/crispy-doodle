using System.ComponentModel.DataAnnotations;

namespace AccountService.Dto.Request
{
    public class UpdateProfileDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Bio {  get; set; }
    }
}

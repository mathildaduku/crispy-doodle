namespace ContentService.Dto.Response
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; }
    }
}

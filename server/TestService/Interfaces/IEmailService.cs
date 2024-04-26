namespace TestService;

public interface IEmailService
{
    public void SendEmail(string from, string to, string subject, string body); 
}

using Microsoft.AspNetCore.Identity.UI.Services;

namespace BuyEverything.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //logic to be send here
            return Task.CompletedTask;
        }
    }
}
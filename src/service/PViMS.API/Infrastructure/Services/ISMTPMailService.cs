using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface ISMTPMailService
    {
        Task SendEmailAsync(string subject, string body, string destinationMailboxName, string destinationMailboxAddress);
    }
}

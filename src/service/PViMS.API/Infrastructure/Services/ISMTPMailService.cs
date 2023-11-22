using MimeKit;
using PVIMS.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface ISMTPMailService
    {
        Task SendEmailAsync(string subject, string body, List<MailboxAddress> destinationAddresses, List<ArtefactInfoModel> attachments);

        bool CheckIfEnabled();
    }
}

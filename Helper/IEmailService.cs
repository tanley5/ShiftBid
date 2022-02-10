using System.Collections.Generic;

namespace Shiftbid.Helper
{
    public interface IEmailService
    {
        void Send(EmailMessage emailMessage);
        List<EmailMessage> ReceiveEmail(int maxCount = 10);
    }
}
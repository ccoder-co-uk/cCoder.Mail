using cCoder.Mail.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Mail;
using cCoder.Data.Models.Security;

namespace cCoder.Mail.Exposures;

public interface IMailManagerExposure
{
    ValueTask<QueuedEmail> AddAsync(QueuedEmail email, bool checkPrivileges = false);
}


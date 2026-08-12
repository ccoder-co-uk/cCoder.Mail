// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models.Exceptions;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MicrosoftGraphMailSenderService
{
    private static async Task TryCatch(Func<Task> operation, bool isTask)
    {
        try
        {
            await operation();
        }
        catch (MailValidationException innerException)
        {
            throw new MailValidationException(innerException: innerException);
        }
        catch (MailDependencyException innerException)
        {
            throw new MailDependencyException(innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new MailValidationException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new MailServiceException(innerException: innerException);
        }
    }
}
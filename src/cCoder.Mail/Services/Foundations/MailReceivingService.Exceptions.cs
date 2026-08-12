// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models.Exceptions;

namespace cCoder.Mail.Services.Foundations;

internal sealed partial class MailReceivingService
{
    private static async Task<TResult> TryCatch<TResult>(
        Func<Task<TResult>> operation,
        bool isTask)
    {
        try
        {
            return await operation();
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
        }    }
}
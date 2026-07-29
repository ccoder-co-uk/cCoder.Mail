// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models.Exceptions;

namespace cCoder.Mail.Providers.Services.Foundations;

internal sealed partial class MailProviderService
{
    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation,
        bool isValueTask)
    {
        try
        {
            return await operation();
        }
        catch (MailValidationException innerException)
        {
            throw new MailValidationException(
                innerException: innerException);
        }
        catch (MailDependencyException innerException)
        {
            throw new MailDependencyException(
                innerException: innerException);
        }
        catch (ArgumentException innerException)
        {
            throw new MailValidationException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new MailServiceException(
                innerException: innerException);
        }
    }
}
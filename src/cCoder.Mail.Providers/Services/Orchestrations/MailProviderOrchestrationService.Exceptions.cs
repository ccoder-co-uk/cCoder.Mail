// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models.Exceptions;

namespace cCoder.Mail.Providers.Services.Orchestrations;

internal sealed partial class MailProviderOrchestrationService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
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
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new MailServiceException(
                innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation)
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
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception innerException)
        {
            throw new MailServiceException(
                innerException: innerException);
        }
    }
}
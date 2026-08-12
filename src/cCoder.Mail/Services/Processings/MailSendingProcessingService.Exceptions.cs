// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Providers.Models.Exceptions;

namespace cCoder.Mail.Services.Processings;

internal sealed partial class MailSendingProcessingService
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

    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
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

    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
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

    private static async Task TryCatch(
        Func<Task> operation,
        bool isTask,
        CancellationToken cancellationToken)
    {
        try
        {
            await operation();
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
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
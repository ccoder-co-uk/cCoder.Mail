// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Eventing.Models;


namespace cCoder.Mail.Brokers.Events;

public interface IQueuedEmailEventBroker
{
    ValueTask RaiseQueuedEmailAddEventAsync(EventMessage<QueuedEmail> message);
    ValueTask RaiseQueuedEmailUpdateEventAsync(EventMessage<QueuedEmail> message);
    ValueTask RaiseQueuedEmailDeleteEventAsync(EventMessage<QueuedEmail> message);
}
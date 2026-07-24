// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Mail;
using cCoder.Eventing.Models;


namespace cCoder.Mail.Brokers.Events;

public interface ISentEmailEventBroker
{
    ValueTask RaiseSentEmailAddEventAsync(EventMessage<SentEmail> message);
    ValueTask RaiseSentEmailUpdateEventAsync(EventMessage<SentEmail> message);
    ValueTask RaiseSentEmailDeleteEventAsync(EventMessage<SentEmail> message);
}
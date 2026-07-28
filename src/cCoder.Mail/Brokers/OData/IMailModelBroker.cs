// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models.OData;

namespace cCoder.Mail.Brokers.OData;

internal interface IMailModelBroker
{
    ODataModel Build();
    void Configure();
}
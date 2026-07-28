// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Models.OData;

namespace cCoder.Mail.Brokers.OData;

internal interface IODataModelBroker
{
    ODataModel Build();
}
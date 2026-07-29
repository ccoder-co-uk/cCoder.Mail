// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail.Brokers.OData;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Mail;

internal static class ODataConventionModelBuilderExtensions
{
    internal static void ConfigureMailApiModel(
        this ODataConventionModelBuilder builder) =>
        new MailModelBroker(builder: builder).Configure();
}
// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Mail;
using cCoder.Mail.Models;

namespace Mail.HostedServices;

public static class WebApplicationExtensions
{
    public static WebApplication UseMailHostedServicesApplication(
        this WebApplication app)
    {
        _ = app.Services.GetRequiredService<MailConfiguration>();

        app.MapGet(
            pattern: "/",
            handler: (IHostEnvironment environment) =>
                Results.Text(
                    content: BuildHostedServicesReport(
                        environment: environment),
                    contentType: "text/plain"));
        app.MapGet(
            pattern: "/Health",
            handler: () => Results.Text(content: "Healthy"));
        app.StartMailHostedServices();

        return app;
    }

    private static string BuildHostedServicesReport(
        IHostEnvironment environment) =>
        string.Join(
            separator: Environment.NewLine,
            "cCoder.Mail Hosted Services",
            "Status: Healthy",
            $"Environment: {environment.EnvironmentName}",
            "Health: /Health",
            string.Empty,
            "Hosted background services:",
            "- MailSenderHostedService",
            "- MailReceiverHostedService",
            string.Empty,
            "Hosted event listeners:",
            "- app_add -> mail app setup",
            "- app_update -> mail app update",
            "- app_delete -> mail app cleanup");
}
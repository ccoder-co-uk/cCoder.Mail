// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Mail.Web.Hosting;

namespace Mail.Web;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args: args);
        builder.Services.AddMailWebApplication(configuration: builder.Configuration);

        WebApplication app = builder.Build();
        app.UseMailWebApplication();
        app.Run();
    }
}
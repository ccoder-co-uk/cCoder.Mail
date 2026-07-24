// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Mail.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Pages =>
        new()
        {
            Name = "Mail Pages",
            Category = "Mail",
            Description = "Mail Pages.",
            SourceApi = "https://ccoder.co.uk/Api/",
            Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/MailManagement",
  "Name": "Mail Management",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.3267878+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing Mail Management </h2><p class=\"mainText\">You access this functionality through the App Management Tabs. From here, you can see any\n        emails that have been sent from our system. These could be password reset emails, registration emails or just\n        the usual registration emails.</p><h2>The UI</h2><p class=\"mainText\">When you access the mail management tab, you&rsquo;re greeted with something that looks like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/MailManagementGrid-en.png\" /><p class=\"mainText\">Here, you would see any emails that were due to be sent out. As this is our demo system there\n        are currently none as not many people use the site from a day to day basis.</p><p class=\"mainText\">&nbsp;</p><h2>History Tab</h2><p class=\"mainText\">In the history tab, you can view any emails that have been sent out by our system previously and\n        it looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/HistoryTab-en.png\" /><p class=\"mainText\">The columns show: </p><ul><li>Subject - the reason why the email was sent, </li><li>From - where the email was sent from, </li><li>To - the email address it was sent to, </li><li>Sent On - the date that the email was sent out to the user. </li></ul><p class=\"mainText\">Expanding one of these rows will display the content of the email that was sent out to the user\n        at that time.</p><p class=\"mainText\">&nbsp;</p><h2>Mail Server Tab </h2><p class=\"mainText\">This tab is for configuring the mail accounts that the app can use to send emails out.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/MailServerTab-en.png\" /><h2>&nbsp;</h2><h3>Creating a Mail Server</h3><p class=\"mainText\">To set up a new in this UI, you click on the <strong>&ldquo;New&rdquo;</strong> button in the header bar of\n        the mail server grid. Clicking this will bring up a dialog that looks something like this:</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/CreateMailServerDialog-en.png\" width=\"681\" height=\"353\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Here, <strong>&ldquo;Name&rdquo;</strong> represents the name you want to give to your mail server and you\n        may want it to be\n        relevant to that app you are in. <strong>&ldquo;User&rdquo;</strong> is the email address you wish the emails to be sent\n        from and <strong>&ldquo;Password&rdquo;</strong>\n is the password for that email account. <strong>&ldquo;Host&rdquo; </strong>is the name of the host which is used to send\n        emails and <strong>&ldquo;Port&rdquo;</strong>\n is the communication endpoint which is usually set to 25. In the <strong>&ldquo;Enable SSL&rdquo;</strong> box, you can\n        either set this to\n        true or false, as you can see it&rsquo;s set as true be default.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/TestMailServer-en.png\" /><p class=\"mainText\">For now, I have created a test mail server. On this row you can see that there are save and\n        delete functions.</p><p class=\"mainText\">&nbsp;</p><h3>Updating a Mail Server </h3><p class=\"mainText\">You edit mail servers by just clicking onto the field you want to change. </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/UpdateMailServer-en.png\" /><p class=\"mainText\">Once you&rsquo;ve made any changes you want, you click the <strong>&ldquo;Save&rdquo; </strong>button on the\n        corresponding grid row and any changes you&rsquo;ve made will be saved.</p><p class=\"mainText\">&nbsp;</p><h3>Deleting a Mail Server </h3><p class=\"mainText\">To delete a mail server that you no longer want to use in this app, then you click the\n        <strong>&ldquo;Remove&rdquo; </strong>button that will remove your schedule and its content from the UI.</p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Mail Management/DeleteMailServer-en.png\" /></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access and manage the mail sending in the portal.",
      "Title": "Mail Management"
    }
  ]
}
"""
            }
        ]
        };
}
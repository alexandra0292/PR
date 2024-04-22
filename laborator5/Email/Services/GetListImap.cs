using Microsoft.Extensions.Options;
using Email.Interfaces;
using Email.Model.mail;
using MailKit.Net.Imap;
using MailKit;
using Email.Model;
using MimeKit;

namespace Email.Services
{
    public class GetListImap : IImapService
    {
        private readonly MailSettings _mailSettings;

        public GetListImap(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<List<Emails>> GetListMailsAsync()
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    int totalMessages = inbox.Count;
                    int startIndex;
                    int endIndex;

                    if (totalMessages > 10)
                    {
                        startIndex = totalMessages - 10;
                        endIndex = totalMessages - 1;
                    }
                    else
                    {
                        startIndex = 0;
                        endIndex = totalMessages - 1;
                    }


                    var emailList = new List<Emails>();


                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        var message = inbox.GetMessage(i);
                        var attachments = new List<FileData>();

                        foreach (var attachment in message.Attachments)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                if (attachment is MimePart mimePart)
                                {
                                    await mimePart.Content.DecodeToAsync(memoryStream);
                                    var fileBytes = memoryStream.ToArray();
                                    var base64String = Convert.ToBase64String(fileBytes);
                                    var file = new FileData();
                                    file.Name = mimePart.ContentDisposition?.FileName ?? "Untitled";
                                    file.ContentType = attachment.ContentType.MimeType;
                                    file.DataBase64 = base64String;
                                    attachments.Add(file);
                                }
                            }
                        }

                        var email = new Emails
                        {
                            Subject = message.Subject,
                            From = message.From.ToString(),
                            Date = message.Date.UtcDateTime,
                            Body = message.TextBody,
                            Attachments = attachments

                        };

                        emailList.Add(email);
                    }
                    emailList.Reverse();

                    client.Disconnect(true);

                    return emailList;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

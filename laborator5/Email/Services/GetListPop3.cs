using Microsoft.Extensions.Options;
using MailKit.Net.Pop3;
using MimeKit;
using Email.Interfaces;
using Email.Model.mail;
using Email.Model;

namespace Email.Services
{
    public class GetListPop3 : IPop3Service
    {
        private readonly MailSettings _mailSettings;

        public GetListPop3(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task<List<Emails>> GetListMailsAsync()
        {
            try
            {
                using (var client = new Pop3Client())
                {
                    client.Connect("pop.gmail.com", 995, true);
                    client.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                    var emailList = new List<Emails>();

                    int totalMessages = client.Count;

                    int startIndex = Math.Max(0, totalMessages - 10);
                    int endIndex = totalMessages - 1;

                    for (int i = endIndex; i >= startIndex; i--)
                    {
                        var message = client.GetMessage(i);
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

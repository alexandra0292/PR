using Microsoft.AspNetCore.Mvc;
using Email.Interfaces;
using Email.Model.mail;
using Email.Model;
using System.Text;

namespace Email.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : ControllerBase
    {
        private readonly IMailsService mailService;
        private readonly IImapService imapService;
        private readonly IPop3Service pop3Service;
        public MailController(IMailsService mailService, IImapService imapService, IPop3Service pop3Service)
        {
            this.mailService = mailService;
            this.imapService = imapService;
            this.pop3Service = pop3Service;

        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await mailService.SendEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpGet("getMailsImap")]
        public async Task<ActionResult<List<Emails>>> GetEmails()
        {
            try
            {
                List<Emails> list = await imapService.GetListMailsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("getMailsPop3")]
        public async Task<ActionResult<List<Emails>>> GetEmailsPop3()
        {
            try
            {
                List<Emails> list = await pop3Service.GetListMailsAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("reply")]
        public async Task<IActionResult> ReplyToEmail([FromForm] MailRequest email)
        {
            
            var replyEmail = new MailRequest
            {
                ToEmail = email.ToEmail,
                Subject = $"Re: {email.Subject}",
                Body = $"{email.Body} Vă mulțumesc pentru răspuns. Cu respect, Alexadra!",
                Attachments = email.Attachments
            };
            try
            {
                await mailService.SendEmailAsync(replyEmail);
                return Ok();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("downloadEmail")]
        public async Task<IActionResult> DownloadEmail([FromQuery] string subject , [FromQuery] string From)
        {
            try
            {
                var emails = await imapService.GetListMailsAsync();

                var email = emails.FirstOrDefault(e => e.Subject == subject && e.From == From);

                if (email == null)
                {
                    return NotFound($"E-mailul cu adresa expeditorului {From} și subiectul '{subject}' nu a fost găsit.");
                }

                var emailInfo = new StringBuilder();
                emailInfo.AppendLine($"Subiect: {email.Subject}");
                emailInfo.AppendLine($"De la: {email.From}");
                emailInfo.AppendLine($"Data: {email.Date}");
                emailInfo.AppendLine($"--------------------------------------------------------");
                emailInfo.AppendLine($"{email.Body}");

                var currentDirectory = Directory.GetCurrentDirectory();
                var folderPath = Path.Combine(currentDirectory, "Down");

                // Curățare folder "Down" și recreare
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, $"Email_{Guid.NewGuid()}.txt");
                await System.IO.File.WriteAllTextAsync(filePath, emailInfo.ToString(), Encoding.UTF8);

                foreach (var attachment in email.Attachments)
                {
                    var attachmentFilePath = Path.Combine(folderPath, attachment.Name);
                    var attachmentBytes = Convert.FromBase64String(attachment.DataBase64);
                    await System.IO.File.WriteAllBytesAsync(attachmentFilePath, attachmentBytes);
                }

                return Ok($"E-mailul a fost descărcat cu succes și salvat în: {filePath}");
            }
            catch (Exception ex)
            {
                return BadRequest("Eroare la descărcarea e-mailului.");
            }
        }



    }
}

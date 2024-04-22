namespace Email.Model
{
    public class Emails
    {
        public string Subject { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public List<FileData> Attachments { get; set; }
    }
}

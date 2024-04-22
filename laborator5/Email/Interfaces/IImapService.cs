using Email.Model;

namespace Email.Interfaces
{
    public interface IImapService
    {
        Task<List<Emails>> GetListMailsAsync();
    }
}

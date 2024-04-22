using Email.Model;

namespace Email.Interfaces
{
    public interface IPop3Service
    {
        Task<List<Emails>> GetListMailsAsync();
    }
}

using Web2FA.Backend.Repository.Interfaces.Derived;

namespace Web2FA.Backend.Repository.Interfaces.Base
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> CommitChangesAsync();
        int CommitChanges();

        IUserRepository UserRepository { get; }
    }
}

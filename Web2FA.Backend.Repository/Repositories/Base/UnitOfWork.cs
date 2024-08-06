using Web2FA.Backend.Model.Models;
using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Repository.Interfaces.Derived;

namespace Web2FA.Backend.Repository.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Web2FAContext context;

        public UnitOfWork(Web2FAContext context, IUserRepository userRepository)
        {
            this.context = context;
            UserRepository = userRepository;
        }

        public IUserRepository UserRepository { get; }

        private bool disposed = false;

        public async Task<int> CommitChangesAsync()
        {
            var id = await context.SaveChangesAsync();
            return id;
        }

        public int CommitChanges()
        {
            var id = context.SaveChanges();
            return id;
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    try
                    {
                        context.Dispose();
                    }
                    catch (Exception)
                    { }
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

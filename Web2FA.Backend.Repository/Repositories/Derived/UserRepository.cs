using Web2FA.Backend.Model.Models;
using Web2FA.Backend.Model.Models.Derived;
using Web2FA.Backend.Repository.Interfaces.Derived;
using Web2FA.Backend.Repository.Repositories.Base;

namespace Web2FA.Backend.Repository.Repositories.Derived
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public sealed class UserRepository : GenericRepositoryBase<User, Web2FAContext>, IUserRepository
    {
        public UserRepository(Web2FAContext pContext) : base(pContext)
        {

        }
    }
}

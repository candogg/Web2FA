using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Service.Interfaces.Base;

namespace Web2FA.Backend.Service.Services.Base
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public abstract class ServiceBase : IServiceBase
    {
        public readonly IUnitOfWork unitOfWork;

        public ServiceBase(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}

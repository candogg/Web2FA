using System.Linq.Expressions;
using Web2FA.Backend.Shared.Filters.Base;

namespace Web2FA.Backend.Repository.Interfaces.Base
{
    public interface IGenericRepository<T> where T : class, new()
    {
        Task<T?> AddAsync(T pEntity);
        Task<List<T>?> AddRangeAsync(List<T> pEntities);
        Task<T?> GetAsync(Expression<Func<T, bool>> pFilter, List<string>? pIncludedEntity = null);
        Task<List<T>?> GetListAsync(Expression<Func<T, bool>> pFilter, List<string>? pIncludedEntity = null, FilterBase? filterItem = null, string orderField = "", bool ascending = true);
        Task<(List<T>?, int)> GetListWithTotalCountAsync(Expression<Func<T, bool>> pFilter, List<string>? pIncludedEntity = null, FilterBase? filterItem = null, string orderField = "", bool ascending = true);
        Task<T?> UpdateAsync(T pEntity);
        Task<List<T>?> UpdateRangeAsync(List<T> pEntities);
    }
}

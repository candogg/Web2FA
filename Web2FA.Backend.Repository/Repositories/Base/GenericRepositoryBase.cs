using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web2FA.Backend.Model.Models;
using Web2FA.Backend.Repository.Interfaces.Base;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Filters.Base;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Backend.Repository.Repositories.Base
{
    /// <summary>
    /// Author: Can DOĞU (CENTECH)
    /// </summary>
    public abstract class GenericRepositoryBase<T, Context> : IGenericRepository<T>
        where T : class, new()
        where Context : Web2FAContext, new()
    {
        private readonly Context _context;

        protected GenericRepositoryBase(Context pContext)
        {
            _context = pContext;
        }

        public async Task<T?> AddAsync(T pEntity)
        {
            try
            {
                var newEntity = await _context.AddAsync(pEntity);
                await _context.SaveChangesAsync();

                return newEntity.Entity;
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }

        public async Task<List<T>?> AddRangeAsync(List<T> pEntities)
        {
            try
            {
                await _context.AddRangeAsync(pEntities);
                await _context.SaveChangesAsync();

                return await Task.FromResult(pEntities);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> pFilter, List<string>? pIncludedEntity = null)
        {
            try
            {
                var oSearch = _context.Set<T>().Where(pFilter).AsQueryable();

                if (pIncludedEntity != null && pIncludedEntity.Count > 0)
                {
                    foreach (var include in pIncludedEntity)
                    {
                        oSearch = oSearch.Include(include);
                    }
                }

                var oResult = await oSearch.FirstOrDefaultAsync();

                return await Task.FromResult(oResult);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }

        public async Task<List<T>?> GetListAsync(Expression<Func<T, bool>> pFilter, List<string>? pIncludedEntity = null, FilterBase? filterItem = null, string orderField = "", bool ascending = true)
        {
            try
            {
                IQueryable<T> oSearch = _context.Set<T>();

                if (pFilter != null)
                {
                    oSearch = oSearch.Where(pFilter);
                }

                if (orderField.IsNotNullOrEmpty())
                {
                    oSearch = oSearch.OrderByPropertyOrField(orderField, ascending);
                }

                if (pIncludedEntity != null && pIncludedEntity.Count > 0)
                {
                    foreach (var include in pIncludedEntity)
                    {
                        oSearch = oSearch.Include(include);
                    }
                }

                if (filterItem != null)
                {
                    var toSkip = filterItem.Page * filterItem.TotalItems;
                    oSearch = oSearch.Skip(toSkip).Take(filterItem.TotalItems);
                }

                return await oSearch.ToListAsync();
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }

        public async Task<(List<T>?, int)> GetListWithTotalCountAsync(Expression<Func<T, bool>> pFilter, List<string>? pIncludedEntity = null, FilterBase? filterItem = null, string orderField = "", bool ascending = true)
        {
            try
            {
                IQueryable<T>? oSearch;

                if (orderField.IsNullOrEmpty())
                {
                    oSearch = _context.Set<T>().Where(pFilter).AsQueryable();
                }
                else
                {
                    oSearch = _context.Set<T>().Where(pFilter).OrderByPropertyOrField(orderField, ascending).AsQueryable();
                }

                if (pIncludedEntity != null && pIncludedEntity.Count > 0)
                {
                    foreach (var include in pIncludedEntity)
                    {
                        oSearch = oSearch.Include(include);
                    }
                }

                var totalCount = await oSearch.CountAsync();

                if (filterItem != null)
                {
                    var toSkip = filterItem.Page * filterItem.TotalItems;

                    oSearch = oSearch.Skip(toSkip).Take(filterItem.TotalItems);
                }

                return (await oSearch.ToListAsync(), totalCount);
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }

        public async Task<T?> UpdateAsync(T pEntity)
        {
            try
            {
                _context.Entry(pEntity).State = EntityState.Modified;

                int oNumberOfRowAffected = await _context.SaveChangesAsync();

                if (oNumberOfRowAffected > 0)
                {
                    return await Task.FromResult(pEntity);
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }

        public async Task<List<T>?> UpdateRangeAsync(List<T> pEntities)
        {
            try
            {
                int oNumberOfRowAffectedTotal = 0;

                foreach (var entity in pEntities)
                {
                    _context.Entry(entity).State = EntityState.Modified;

                    int oNumberOfRowAffected = await _context.SaveChangesAsync();

                    if (oNumberOfRowAffected > 0) oNumberOfRowAffectedTotal += oNumberOfRowAffected;
                }

                if (oNumberOfRowAffectedTotal == pEntities.Count)
                {
                    return await Task.FromResult(pEntities);
                }
                else
                {
                    return default;
                }
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);

                return default;
            }
        }
    }
}

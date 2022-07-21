using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Records.Bases;
using AppCoreLite.Results;
using AppCoreLite.Results.Bases;
using AppCoreLite.Services.Bases;
using AppCoreLite.Utils.Bases.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Services
{
    /// <summary>
    /// Service class for managing data access CRUD operations using entities.
    /// </summary>
    public class EntityService<T> : ServiceBase<T> where T : Record, new()
    {
        public EntityService(DbContext db, UserUtilBase? userUtil, SessionUtilBase? sessionUtil, RecordFileUtilBase? fileUtil) : base(db, userUtil, sessionUtil, fileUtil)
        {
            _propertiesForOrdering = _reflectionUtil.GetProperties<T>(TagAttributes.Order);
            _propertiesForFiltering = _reflectionUtil.GetProperties<T>(TagAttributes.StringFilter);
        }

        public virtual IQueryable<T> Query(params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            var query = _db.Set<T>().AsQueryable();
            if (_isDeleted != null)
            {
                query = query.Where(q => (EF.Property<bool?>(q, _isDeleted) ?? false) == false).AsQueryable();
            }
            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                query = query.Include(navigationPropertyPath);
            }
            return query;
        }

        public virtual List<T> GetList(params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            var list = Query(navigationPropertyPaths).ToList();
            var count = list.Count;
            TotalRecordsCount = count == 0 ? Config.RecordNotFound
                : count == 1 ? (count + " " + Config.RecordFound).ToLower()
                : (count + " " + Config.RecordsFound).ToLower();
            return list;
        }

        public virtual List<T> GetList(Expression<Func<T, bool>> predicate, params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            var list = Query(navigationPropertyPaths).Where(predicate).ToList();
            var count = list.Count;
            TotalRecordsCount = count == 0 ? Config.RecordNotFound
                : count == 1 ? (count + " " + Config.RecordFound).ToLower()
                : (count + " " + Config.RecordsFound).ToLower();
            return list;
        }

        public virtual List<T> GetList(PageOrderFilter pageOrderFilter, bool usePageOrderFilterSession = false, params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            var query = Query(navigationPropertyPaths);
            var sessionKey = this.ToString()?.Split('.').LastOrDefault() + Config.PageOrderFilterSessionKey;
            var pageOrderFilterSession = _sessionUtil?.GetSessionObject<PageOrderFilter>(sessionKey);
            if (usePageOrderFilterSession && pageOrderFilterSession != null)
            {
                pageOrderFilter.PageNumber = pageOrderFilterSession.PageNumber;
                pageOrderFilter.RecordsPerPageCount = pageOrderFilterSession.RecordsPerPageCount;
                pageOrderFilter.OrderExpression = pageOrderFilterSession.OrderExpression;
                pageOrderFilter.IsOrderDirectionAscending = pageOrderFilterSession.IsOrderDirectionAscending;
                pageOrderFilter.Filter = pageOrderFilterSession.Filter;
            }
            if (pageOrderFilter != null)
            {
                var propertyForOrdering = GetProperty(pageOrderFilter.OrderExpression);
                if (propertyForOrdering != null)
                {
                    query = pageOrderFilter.IsOrderDirectionAscending ? query.OrderBy(_reflectionUtil.GetOrderExpression<T>(propertyForOrdering.Name))
                            : query.OrderByDescending(_reflectionUtil.GetOrderExpression<T>(propertyForOrdering.Name));
                }
                if (_propertiesForFiltering != null && _propertiesForFiltering.Count > 0 && !string.IsNullOrWhiteSpace(pageOrderFilter.Filter))
                {
                    var predicate = _reflectionUtil.GetPredicateContainsExpression<T>(_propertiesForFiltering[0].Name, pageOrderFilter.Filter);
                    for (var i = 1; i < _propertiesForFiltering.Count; i++)
                    {
                        predicate = predicate.Or(_reflectionUtil.GetPredicateContainsExpression<T>(_propertiesForFiltering[i].Name, pageOrderFilter.Filter));
                    }
                    query = query.Where(predicate);
                }
                pageOrderFilter.TotalRecordsCountResult = query.Count();
                TotalRecordsCount = pageOrderFilter.TotalRecordsCountResult == 0 ? Config.RecordNotFound
                    : pageOrderFilter.TotalRecordsCountResult == 1 ? (pageOrderFilter.TotalRecordsCountResult + " " + Config.RecordFound).ToLower()
                    : (pageOrderFilter.TotalRecordsCountResult + " " + Config.RecordsFound).ToLower();
                RecordsPerPageCounts = Config.RecordsPerPageCounts;
                _pageOrderFilter = pageOrderFilter;
                if (_pageOrderFilter.PageNumber == PageNumbers.LastOrDefault() + 1 && _pageOrderFilter.TotalRecordsCountResult % Convert.ToInt32(_pageOrderFilter.RecordsPerPageCount) == 0)
                {
                    if (_pageOrderFilter.PageNumber > 1)
                        pageOrderFilter.PageNumber--;
                }
                if (RecordsPerPageCounts != null && RecordsPerPageCounts.Count > 0 && pageOrderFilter.RecordsPerPageCount != RecordsPerPageCounts.LastOrDefault())
                {
                    query = query.Skip((pageOrderFilter.PageNumber - 1) * Convert.ToInt32(pageOrderFilter.RecordsPerPageCount))
                            .Take(Convert.ToInt32(pageOrderFilter.RecordsPerPageCount));
                }
            }
            _sessionUtil?.SetSessionObject(sessionKey, pageOrderFilter);
            return query.ToList();
        }

        public virtual List<T> GetList(Expression<Func<T, bool>> predicate, PageOrderFilter pageOrderFilter, bool usePageOrderFilterSession = false, params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            var query = Query(navigationPropertyPaths);
            var sessionKey = this.ToString()?.Split('.').LastOrDefault() + Config.PageOrderFilterSessionKey;
            var pageOrderFilterSession = _sessionUtil?.GetSessionObject<PageOrderFilter>(sessionKey);
            if (usePageOrderFilterSession && pageOrderFilterSession != null)
            {
                pageOrderFilter.PageNumber = pageOrderFilterSession.PageNumber;
                pageOrderFilter.RecordsPerPageCount = pageOrderFilterSession.RecordsPerPageCount;
                pageOrderFilter.OrderExpression = pageOrderFilterSession.OrderExpression;
                pageOrderFilter.IsOrderDirectionAscending = pageOrderFilterSession.IsOrderDirectionAscending;
                pageOrderFilter.Filter = pageOrderFilterSession.Filter;
            }
            if (pageOrderFilter != null)
            {
                var propertyForOrdering = GetProperty(pageOrderFilter.OrderExpression);
                if (propertyForOrdering != null)
                {
                    query = pageOrderFilter.IsOrderDirectionAscending ? query.OrderBy(_reflectionUtil.GetOrderExpression<T>(propertyForOrdering.Name))
                            : query.OrderByDescending(_reflectionUtil.GetOrderExpression<T>(propertyForOrdering.Name));
                }
                if (_propertiesForFiltering != null && _propertiesForFiltering.Count > 0 && !string.IsNullOrWhiteSpace(pageOrderFilter.Filter))
                {
                    predicate = _reflectionUtil.GetPredicateContainsExpression<T>(_propertiesForFiltering[0].Name, pageOrderFilter.Filter);
                    for (var i = 1; i < _propertiesForFiltering.Count; i++)
                    {
                        predicate = predicate.Or(_reflectionUtil.GetPredicateContainsExpression<T>(_propertiesForFiltering[i].Name, pageOrderFilter.Filter));
                    }
                    query = query.Where(predicate);
                }
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (pageOrderFilter != null)
            {
                pageOrderFilter.TotalRecordsCountResult = query.Count();
                TotalRecordsCount = pageOrderFilter.TotalRecordsCountResult == 0 ? Config.RecordNotFound
                    : pageOrderFilter.TotalRecordsCountResult == 1 ? (pageOrderFilter.TotalRecordsCountResult + " " + Config.RecordFound).ToLower()
                    : (pageOrderFilter.TotalRecordsCountResult + " " + Config.RecordsFound).ToLower();
                RecordsPerPageCounts = Config.RecordsPerPageCounts;
                _pageOrderFilter = pageOrderFilter;
                if (_pageOrderFilter.PageNumber == PageNumbers.LastOrDefault() + 1 && _pageOrderFilter.TotalRecordsCountResult % Convert.ToInt32(_pageOrderFilter.RecordsPerPageCount) == 0)
                {
                    if (_pageOrderFilter.PageNumber > 1)
                        pageOrderFilter.PageNumber--;
                }
                if (RecordsPerPageCounts != null && RecordsPerPageCounts.Count > 0 && pageOrderFilter.RecordsPerPageCount != RecordsPerPageCounts.LastOrDefault())
                {
                    query = query.Skip((pageOrderFilter.PageNumber - 1) * Convert.ToInt32(pageOrderFilter.RecordsPerPageCount))
                            .Take(Convert.ToInt32(pageOrderFilter.RecordsPerPageCount));
                }
            }
            _sessionUtil?.SetSessionObject(sessionKey, pageOrderFilter);
            return query.ToList();
        }

        public virtual T? GetItem(Expression<Func<T, bool>> predicate, params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            return Query(navigationPropertyPaths).SingleOrDefault(predicate);
        }

        public virtual T? GetItem(int id, params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            return Query(navigationPropertyPaths).SingleOrDefault(q => q.Id == id);
        }

        public virtual Result ItemExists(Expression<Func<T, bool>> predicate, params Expression<Func<T, object?>>[] navigationPropertyPaths)
        {
            bool exists = Query(navigationPropertyPaths).Any(predicate);
            return exists ? new SuccessResult(Config.RecordFound) : new ErrorResult(Config.RecordNotFound);
        }

        public virtual Result Add(T entity, bool save = true, bool trim = true)
        {
            if (_fileUtil != null && _fileUtil.CheckFile() == false)
                return new ErrorResult(Config.InvalidFileExtensionOrFileLength);
            if (trim)
                _reflectionUtil.TrimStringProperties(entity);
            _db.Add(entity);
            if (save)
            {
                Save();
                _fileUtil?.SaveFile(entity.Id);
                return new SuccessResult(Config.AddedSuccessfuly);
            }
            return new ErrorResult(Config.ChangesNotSaved);
        }

        public virtual Result Update(T entity, bool save = true, bool trim = true)
        {
            if (_fileUtil != null && _fileUtil.CheckFile() == false)
                return new ErrorResult(Config.InvalidFileExtensionOrFileLength);
            if (trim)
                _reflectionUtil.TrimStringProperties(entity);
            _db.Update(entity);
            if (save)
            {
                Save();
                _fileUtil?.SaveFile(entity.Id);
                return new SuccessResult(Config.UpdatedSuccessfuly);
            }
            return new ErrorResult(Config.ChangesNotSaved);
        }

        public virtual Result Delete(T entity, bool save = true)
        {
            _db.Remove(entity);
            if (save)
            {
                Save();
                return new SuccessResult(Config.DeletedSuccessfuly);
            }
            return new ErrorResult(Config.ChangesNotSaved);
        }

        public virtual Result Delete(Expression<Func<T, bool>> predicate, bool save = true)
        {
            var entities = _db.Set<T>().Where(predicate).ToList();
            foreach (var entity in entities)
            {
                Delete(entity, false);
            }
            if (save)
            {
                Save();
                return new SuccessResult(Config.DeletedSuccessfuly);
            }
            return new ErrorResult(Config.ChangesNotSaved);
        }

        public virtual Result Delete(List<int> ids, bool save = true)
        {
            var entities = _db.Set<T>().Where(e => ids.Contains(e.Id)).ToList();
            foreach (var entity in entities)
            {
                Delete(entity, false);
            }
            if (save)
            {
                Save();
                return new SuccessResult(Config.DeletedSuccessfuly);
            }
            return new ErrorResult(Config.ChangesNotSaved);
        }

        public virtual Result Delete(int id, bool save = true)
        {
            return Delete(new List<int>() { id }, save);
        }
    }
}

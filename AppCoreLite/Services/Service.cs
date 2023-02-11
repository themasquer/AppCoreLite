using AppCoreLite.Enums;
using AppCoreLite.Models;
using AppCoreLite.Records.Bases;
using AppCoreLite.Results.Bases;
using AppCoreLite.Services.Bases;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AppCoreLite.Services
{
    /// <summary>
    /// Service class for managing data access CRUD operations using models.
    /// </summary>
    public class Service<TEntity, TModel> : ServiceBase<TEntity> where TEntity : Record, new() where TModel : Record, new()
    {
        private Mapper? _mapper;
        private MapperConfiguration? _mapperConfiguration;

        public Service(DbContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
            _propertiesForOrdering = _reflectionUtil.GetProperties<TModel>(TagAttributes.Order);
            _propertiesForFiltering = _reflectionUtil.GetProperties<TModel>(TagAttributes.StringFilter);
            Set(new MapperConfiguration(c =>
            {
                c.CreateMap<TEntity, TModel>().ReverseMap();
            }));
        }

        protected void Set(MapperConfiguration mapperConfiguration, Languages language = Languages.Turkish)
        {
            _mapperConfiguration = mapperConfiguration;
            _mapper = new Mapper(mapperConfiguration);
            base.Set(language);
        }

        public virtual IQueryable<TModel> Query(params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            var query = _db.Set<TEntity>().AsQueryable();
            if (_isDeleted != null)
            {
                query = query.Where(q => (EF.Property<bool?>(q, _isDeleted) ?? false) == false).AsQueryable();
            }
            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                query = query.Include(navigationPropertyPath);
            }
            return query.ProjectTo<TModel>(_mapperConfiguration);
        }

        public virtual IQueryable<TModel> Query(bool includeDeleted, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            var query = _db.Set<TEntity>().AsQueryable();
            if (_isDeleted != null && !includeDeleted)
            {
                query = query.Where(q => (EF.Property<bool?>(q, _isDeleted) ?? false) == false).AsQueryable();
            }
            foreach (var navigationPropertyPath in navigationPropertyPaths)
            {
                query = query.Include(navigationPropertyPath);
            }
            return query.ProjectTo<TModel>(_mapperConfiguration);
        }

        public virtual List<TModel> GetList(params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            var list = Query(navigationPropertyPaths).ToList();
            var count = list.Count;
            TotalRecordsCount = count == 0 ? Config.RecordNotFound
                : count == 1 ? (count + " " + Config.RecordFound).ToLower()
                : (count + " " + Config.RecordsFound).ToLower();
            return list;
        }

        public virtual List<TModel> GetList(Expression<Func<TModel, bool>> predicate, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            var list = Query(navigationPropertyPaths).Where(predicate).ToList();
            var count = list.Count;
            TotalRecordsCount = count == 0 ? Config.RecordNotFound
                : count == 1 ? (count + " " + Config.RecordFound).ToLower()
                : (count + " " + Config.RecordsFound).ToLower();
            return list;
        }

        public virtual List<TModel> GetList(PageOrderFilter pageOrderFilter, bool usePageOrderFilterSession = false, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
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
                var propertyForOrdering = GetOrderingProperty(pageOrderFilter.OrderExpression);
                if (propertyForOrdering != null)
                {
                    query = pageOrderFilter.IsOrderDirectionAscending ? query.OrderBy(_reflectionUtil.GetOrderExpression<TModel>(propertyForOrdering.Name))
                            : query.OrderByDescending(_reflectionUtil.GetOrderExpression<TModel>(propertyForOrdering.Name));
                }
                if (_propertiesForFiltering != null && _propertiesForFiltering.Count > 0 && !string.IsNullOrWhiteSpace(pageOrderFilter.Filter))
                {
                    var predicate = _reflectionUtil.GetPredicateContainsExpression<TModel>(_propertiesForFiltering[0].Name, pageOrderFilter.Filter);
                    for (var i = 1; i < _propertiesForFiltering.Count; i++)
                    {
                        predicate = predicate.Or(_reflectionUtil.GetPredicateContainsExpression<TModel>(_propertiesForFiltering[i].Name, pageOrderFilter.Filter));
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

        public virtual List<TModel> GetList(Expression<Func<TModel, bool>> predicate, PageOrderFilter pageOrderFilter, bool usePageOrderFilterSession = false, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
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
                var propertyForOrdering = GetOrderingProperty(pageOrderFilter.OrderExpression);
                if (propertyForOrdering != null)
                {
                    query = pageOrderFilter.IsOrderDirectionAscending ? query.OrderBy(_reflectionUtil.GetOrderExpression<TModel>(propertyForOrdering.Name))
                            : query.OrderByDescending(_reflectionUtil.GetOrderExpression<TModel>(propertyForOrdering.Name));
                }
                if (_propertiesForFiltering != null && _propertiesForFiltering.Count > 0 && !string.IsNullOrWhiteSpace(pageOrderFilter.Filter))
                {
                    predicate = _reflectionUtil.GetPredicateContainsExpression<TModel>(_propertiesForFiltering[0].Name, pageOrderFilter.Filter);
                    for (var i = 1; i < _propertiesForFiltering.Count; i++)
                    {
                        predicate = predicate.Or(_reflectionUtil.GetPredicateContainsExpression<TModel>(_propertiesForFiltering[i].Name, pageOrderFilter.Filter));
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

        public virtual TModel? GetItem(Expression<Func<TModel, bool>> predicate, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            return Query(navigationPropertyPaths).SingleOrDefault(predicate);
        }

        public virtual TModel? GetItem(int id, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            return Query(navigationPropertyPaths).SingleOrDefault(q => q.Id == id);
        }

        public virtual Result ItemExists(Expression<Func<TModel, bool>> predicate, params Expression<Func<TEntity, object?>>[] navigationPropertyPaths)
        {
            bool exists = Query(navigationPropertyPaths).Any(predicate);
            return exists ? Success(Config.RecordFound) : Error(Config.RecordNotFound);
        }

        public virtual int GetMaxId(bool includeDeleted = false)
        {
            return Query(includeDeleted).Max(q => q.Id);
        }

        public virtual Result Add(TModel model, bool save = true, bool trim = true)
        {
            if (_fileUtil != null && _fileUtil.CheckFile() == false)
                return Error(Config.InvalidFileExtensionOrFileLength);
            if (_mapper != null)
            {
                var entity = _mapper.Map<TEntity>(model);
                if (trim)
                    _reflectionUtil.TrimStringProperties(entity);
                _db.Add(entity);
                if (save)
                {
                    Save();
                    _fileUtil?.SaveFile(entity.Id);
                    return Success(Config.AddedSuccessfuly);
                }
            }
            return Error(Config.ChangesNotSaved);
        }

        public virtual Result Update(TModel model, bool save = true, bool trim = true)
        {
            if (_fileUtil != null && _fileUtil.CheckFile() == false)
                return Error(Config.InvalidFileExtensionOrFileLength);
            if (_mapper != null)
            {
                var entity = _mapper.Map<TEntity>(model);
                if (trim)
                    _reflectionUtil.TrimStringProperties(entity);
                _db.Update(entity);
                if (save)
                {
                    Save();
                    _fileUtil?.SaveFile(entity.Id);
                    return Success(Config.UpdatedSuccessfuly);
                }
            }
            return Error(Config.ChangesNotSaved);
        }

        public virtual Result Delete(int id, bool save = true)
        {
            return Delete(new List<int>() { id }, save);
        }

        public virtual Result Delete(List<int> ids, bool save = true)
        {
            var entities = _db.Set<TEntity>().Where(e => ids.Contains(e.Id)).ToList();
            foreach (var entity in entities)
            {
                Delete(entity, false);
            }
            if (save)
            {
                Save();
                return Success(Config.DeletedSuccessfuly);
            }
            return Error(Config.ChangesNotSaved);
        }

        private Result Delete(TEntity entity, bool save = true)
        {
            _db.Remove(entity);
            if (save)
            {
                Save();
                DeleteFile(entity.Id);
                return Success(Config.DeletedSuccessfuly);
            }
            return Error(Config.ChangesNotSaved);
        }
    }
}

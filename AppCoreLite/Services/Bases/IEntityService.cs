using AppCoreLite.Records.Bases;
using AppCoreLite.Results.Bases;
using System.Linq.Expressions;

namespace AppCoreLite.Services.Bases
{
    /// <summary>
    /// Base entity service interface that includes common methods for its implemented class EntityService.
    /// </summary>
    public interface IEntityService<TEntity> : IDisposable where TEntity : Record, new()
    {
        int Save();

        IQueryable<TEntity> Query(params Expression<Func<TEntity, object?>>[] navigationPropertyPaths);

        Result Add(TEntity entity, bool save = true, bool trim = true);

        Result Update(TEntity entity, bool save = true, bool trim = true);

        Result Delete(List<int> ids, bool save = true);
    }
}

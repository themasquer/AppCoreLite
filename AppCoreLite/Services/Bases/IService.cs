using AppCoreLite.Records.Bases;
using AppCoreLite.Results.Bases;
using System.Linq.Expressions;

namespace AppCoreLite.Services.Bases
{
    /// <summary>
    /// Base model service interface that includes common methods for its implemented class Service.
    /// </summary>
    public interface IService<TEntity, TModel> : IDisposable where TEntity : Record, new() where TModel : Record, new()
    {
        int Save();

        IQueryable<TModel> Query(params Expression<Func<TEntity, object?>>[] navigationPropertyPaths);

        Result Add(TModel model, bool save = true, bool trim = true);

        Result Update(TModel model, bool save = true, bool trim = true);

        Result Delete(List<int> ids, bool save = true);
    }
}

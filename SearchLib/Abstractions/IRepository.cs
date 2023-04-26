using System.Linq.Expressions;

namespace AuraSearch.Abstractions
{
    public interface IRepository<T> where T : IEntity
    {

        Task<T> GetById(string id, CancellationToken cancellationToken = default);

        Task RemoveById(string id, CancellationToken cancellationToken = default);

        Task Update(T entity, CancellationToken cancellationToken = default);

        Task Insert(T entity, CancellationToken cancellationToken = default);

        Task<IQueryable<T>> GetAll(CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> FindByPredicate(Expression<Func<T, bool>> expression, int skip = 0, CancellationToken cancellationToken = default);

        Task<T> GetOneByPredicateAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);



    }
}

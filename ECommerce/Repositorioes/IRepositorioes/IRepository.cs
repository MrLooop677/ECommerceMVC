using System.Linq.Expressions;

namespace ECommerce.Repositorioes.IRepositorioes
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellation = default);

        void Update(T entity);

        void Delete(T entity);

        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, Object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellation = default
            );

        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, Object>>[]? includes = null, bool tracked = true, CancellationToken cancellation = default);
        Task CommitAsync(CancellationToken cancellation = default);

    }
}

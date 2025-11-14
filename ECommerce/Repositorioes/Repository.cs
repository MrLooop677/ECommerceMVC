using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Repositorioes
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private ApplicationDbContext _context;//= new();
        private DbSet<T> _db;


        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task AddAsync(T entity, CancellationToken cancellation = default)
        {
            await _db.AddAsync(entity, cancellation);
        }
        public void Update(T entity)
        {
            _db.Update(entity);

        }
        public void Delete(T entity)
        {
            _db.Remove(entity);
        }
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, Object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellation = default
            )
        {
            var entities = _db.AsQueryable();
            if (expression is not null)
                entities = entities.Where(expression);

            if (includes is not null)
                foreach (var item in includes)
                {
                    entities = entities.Include(item);
                }
            if (!tracked)
                entities = entities.AsNoTracking();
            return await entities.ToListAsync(cancellation);

        }
        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Expression<Func<T, Object>>[]? includes = null, bool tracked = true, CancellationToken cancellation = default)
        {
            return (await GetAsync(expression, includes, tracked, cancellation)).FirstOrDefault();
        }
        public async Task CommitAsync(CancellationToken cancellation = default)
        {
            try
            {
                await _context.SaveChangesAsync(cancellation);

            }
            catch (Exception EX)
            {

                Console.WriteLine($"Error: {EX.Message}");
            }
        }
    }
}

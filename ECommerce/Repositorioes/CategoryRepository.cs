using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Repositorioes
{
    public class CategoryRepository
    {
        ApplicationDbContext _db = new();
        public async Task AddAsync(Category category, CancellationToken cancellation = default)
        {
            await _db.Categories.AddAsync(category);
        }
        public void Update(Category category)
        {
            _db.Categories.Update(category);

        }
        public void Delete(Category category)
        {
            _db.Categories.Remove(category);
        }
        public async Task<IEnumerable<Category>> GetAsync(
            Expression<Func<Category, bool>>? expression = null,
            Expression<Func<Category, Object>>[]? includes = null,
            bool tracked = true,
            CancellationToken cancellation = default
            )
        {
            var categories = _db.Categories.AsQueryable();
            if (expression is not null)
                categories = categories.Where(expression);

            if (includes is not null)
                foreach (var item in includes)
                {
                    categories = categories.Include(item);
                }
            if (!tracked)
                categories = categories.AsNoTracking();
            return await categories.ToListAsync(cancellation);

        }
        public async Task<Category?> GetOneAsync(Expression<Func<Category, bool>>? expression = null, Expression<Func<Category, Object>>[]? includes = null, bool tracked = true, CancellationToken cancellation = default)
        {
            return (await GetAsync(expression, includes, tracked, cancellation)).FirstOrDefault();
        }
        public async Task CommitAsync(CancellationToken cancellation = default)
        {
            try
            {
                await _db.SaveChangesAsync(cancellation);

            }
            catch (Exception EX)
            {

                Console.WriteLine($"Error: {EX.Message}");
            }
        }
    }
}

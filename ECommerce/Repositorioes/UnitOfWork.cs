using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Repositorioes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;


        public UnitOfWork(
            IProductRepository productRepository,
            IProductColorRepository productColorRepository,
            IRepository<Category> categoryRepository,
            IRepository<Brand> brandRepository,
            IRepository<ProductSubImage> productSubImageRepository,
            ApplicationDbContext context
            )
        {
            ProductRepository = productRepository;
            ProductColorRepository = productColorRepository;
            CategoryRepository = categoryRepository;
            BrandRepository = brandRepository;
            ProductSubImageRepository = productSubImageRepository;
            _context = context;
        }

        public IProductRepository ProductRepository { get; }

        public IProductColorRepository ProductColorRepository { get; }

        public IRepository<Category> CategoryRepository { get; }

        public IRepository<Brand> BrandRepository { get; }

        public IRepository<ProductSubImage> ProductSubImageRepository { get; }

        public void Dispose()
        {
            _context.Dispose();
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
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction!.RollbackAsync();
        }
    }
}

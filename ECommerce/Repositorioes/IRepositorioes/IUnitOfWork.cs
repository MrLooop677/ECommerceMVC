namespace ECommerce.Repositorioes.IRepositorioes
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository ProductRepository { get; }
        IProductColorRepository ProductColorRepository { get; }
        IRepository<Category> CategoryRepository { get; }
        IRepository<Brand> BrandRepository { get; }
        IRepository<ProductSubImage> ProductSubImageRepository { get; }
        Task CommitAsync(CancellationToken cancellation = default);
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();


    }
}

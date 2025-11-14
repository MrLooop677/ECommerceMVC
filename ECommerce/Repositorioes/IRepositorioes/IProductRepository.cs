namespace ECommerce.Repositorioes.IRepositorioes
{
    public interface IProductRepository : IRepository<Product>
    {
        Task AddRangeAsync(IEnumerable<Product> products, CancellationToken cancellationToken);


    }
}

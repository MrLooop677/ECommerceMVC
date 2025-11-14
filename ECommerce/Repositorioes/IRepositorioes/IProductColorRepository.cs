namespace ECommerce.Repositorioes.IRepositorioes
{
    public interface IProductColorRepository : IRepository<ProductColor>
    {
        void RemoveRange(IEnumerable<ProductColor> productColors);

    }
}

namespace ECommerce.Repositorioes
{
    public class ProductColorRepository : Repository<ProductColor>
    {
        private ApplicationDbContext _context = new();
        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.RemoveRange(productColors);
        }
    }
}

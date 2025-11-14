namespace ECommerce.Repositorioes
{
    public class ProductColorRepository : Repository<ProductColor>, IProductColorRepository
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        public ProductColorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.RemoveRange(productColors);
        }
    }
}

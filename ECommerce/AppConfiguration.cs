using Microsoft.EntityFrameworkCore;

namespace ECommerce
{
    public static class AppConfiguration
    {
        public static void RegisterConfig(this IServiceCollection services, string connectionString)
        {

            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(connectionString);
            });
            // Add services to the container.
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Brand>, Repository<Brand>>();
            services.AddScoped<IRepository<Product>, Repository<Product>>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IRepository<ProductSubImage>, Repository<ProductSubImage>>();
            services.AddScoped<IProductColorRepository, ProductColorRepository>();
        }
    }
}

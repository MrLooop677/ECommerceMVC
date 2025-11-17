using ECommerce.DataAccess.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSubImage> ProductSubImages { get; set; }
        //override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=PC-46;Initial Catalog=ECommerce519;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.Property(x => x.Description).HasMaxLength(500);
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductColorEntityTypeConfiguration).Assembly);
            base.OnModelCreating(modelBuilder);


        }
        public DbSet<ECommerce.ViewModels.RegisterVM> RegisterVM { get; set; } = default!;
    }
}
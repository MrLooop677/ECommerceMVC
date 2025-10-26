using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.DataAccess.EntityConfigurations
{
    public class ProductSubImgEntityTypeConfiguration : IEntityTypeConfiguration<ProductSubImage>
    {
        public void Configure(EntityTypeBuilder<ProductSubImage> builder)
        {
            builder.HasKey(ps => new { ps.ProductId, ps.SubImg });
            builder.HasOne(ps => ps.Product)
                .WithMany(p => p.ProductSubImages)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

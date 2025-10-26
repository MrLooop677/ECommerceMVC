namespace ECommerce.Models
{
    //[PrimaryKey(nameof(ProductId),nameof(SubImg))]
    public class ProductSubImage
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; } = null;

        public string SubImg { get; set; } = string.Empty;
    }
}

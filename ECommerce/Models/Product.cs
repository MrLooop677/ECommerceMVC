namespace ECommerce.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public decimal price { get; set; }
        public decimal Discount { get; set; }

        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null;
        public int BrandId { get; set; }
        public Brand? Brand { get; set; } = null;
        public List<ProductColor> ProductColors { get; set; }
        public List<ProductSubImage> ProductSubImages { get; set; } = null!;
    }
}

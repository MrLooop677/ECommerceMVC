using ECommerce.Validation;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.ViewModels
{
    public class UpdateBrandRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "هذا الحقل مطلوب")]

        [CustomLengthAttribute(3, max: 100, ErrorMessage = "الاسم يجب أن يكون بين 3 و 100 حرف")]
        public string Name { get; set; } = string.Empty;
        [MaxLength(10)]

        public string? Description { get; set; }
        public bool Status { get; set; }
        public string? Img { get; set; }
        public IFormFile? NewImg { get; set; }
    }
}

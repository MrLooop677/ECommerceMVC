using System.ComponentModel.DataAnnotations;

namespace ECommerce.Validation
{
    //[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomLengthAttribute : ValidationAttribute
    {
        private int _min;
        private int _max;

        public CustomLengthAttribute(int min, int max)
        {
            _min = min;
            _max = max;
        }
        public override bool IsValid(object? value)
        {
            if (value is string result && result.Length >= _min && result.Length <= _max)
            {
                return true;
            }
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
                return ErrorMessage;

            // غير كده استخدم الرسالة الافتراضية
            return $"this {name} must be between {_min} and {_max}";
        }
    }
}

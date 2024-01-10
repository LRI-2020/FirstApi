using System.ComponentModel.DataAnnotations;

namespace FirstApi.ValidationAttributes;

public class MinValueAttribute : ValidationAttribute
{
    private readonly int minValue;

    public MinValueAttribute(int minValue)
    {
        this.minValue = minValue;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        Double input = Convert.ToDouble(value);

        if (!(input < minValue)) return ValidationResult.Success;
        var error = $"{input} should be >= {minValue}";
        return new ValidationResult(error);
    }
}
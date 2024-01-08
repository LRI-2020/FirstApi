using System.ComponentModel.DataAnnotations;

namespace FirstApi.ValidationAttributes;

public class MinValueAttribute : ValidationAttribute
{
    private readonly int value;

    public MinValueAttribute(int value)
    {
        this.value = value;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        Double input = (Double)validationContext.ObjectInstance;

        if (!(input < this.value)) return ValidationResult.Success;
        var error = $"{input} should be >= {this.value}";
        return new ValidationResult(error);

    }

    
}
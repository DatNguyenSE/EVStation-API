using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        public MinimumAgeAttribute(int minAge)
        {
            _minAge = minAge;
            ErrorMessage = $"Tuổi phải lớn hơn hoặc bằng {_minAge}";
        }
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth > DateTime.Today.AddYears(-age))
                    age--;

                if (age < _minAge)
                    return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
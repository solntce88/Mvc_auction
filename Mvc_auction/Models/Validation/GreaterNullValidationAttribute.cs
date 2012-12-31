using System;
using System.ComponentModel.DataAnnotations;
using Mvc_auction.Models.Validation;

namespace Mvc_auction
{
    public class GreaterNullValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsValid(value))
                return ValidationResult.Success;
            return new ValidationResult("Error!Value should be greater then 0!");
        }

        public override bool IsValid(object value)
        {
            if (value!=null)
            {
                return (double)value > 0;
            }
            return false;
        }
    }

}
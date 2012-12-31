using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Mvc_auction.Models.Validation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PriceValidationAttribute : ValidationAttribute
    {
     //   private double minPrice=0;
        public PriceValidationAttribute()
            : base("The price is not valid")
        {
          
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int id = ((LotEditModel)validationContext.ObjectInstance).Id;
            double oldPrice = new LotRepository().GetPrice(id);
            return (double)value > oldPrice ? null : new ValidationResult("Your rate should be > current rate");
        }
    }
}
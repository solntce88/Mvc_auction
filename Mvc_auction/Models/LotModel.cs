using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Mvc_auction.Models.Validation;

namespace Mvc_auction.Models
{
    public class NewLot : Lot, IValidatableObject
    {
        [DisplayName("Lot Id")]
        public override int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please, enter the name of Lot")]
        public override string Name { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public override string Description { get; set; }

        [DisplayName("Price")]
        [Required]
        [GreaterNullValidation]
        public override double Price { get; set; }

        [DisplayName("Picture")]
        public override string Picture { get; set; }

        [DisplayName("Start time of auction for this lot")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public override DateTime StartTime { get; set; }

        [DisplayName("End time of auction for this lot")]
        [Required]
        public override DateTime DateEnd { get; set; }

        [DisplayName("Owner")]
        public override int Owner_id { get; set; }

        [DisplayName("Category")]
        public override int Category_id { get; set; }

        public NewLot()
        {
            Category_id = 1;
            DateEnd = DateTime.Now.AddDays(1);
            StartTime = DateTime.Now.AddMinutes(10);
            Price = 0;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // поля для которых реализована custom validation
            var end = new[] { "DateEnd" };
            var start = new[] { "StartTime" };

            if (DateEnd <= DateTime.Now)
            {
                yield return new ValidationResult("Date end of auction can't be in past", end);
            }
            if (DateEnd > DateTime.Now.AddDays(7))
            {
                yield return new ValidationResult("Date end of auction can't be more then week", end);
            }

            if (StartTime < DateTime.Now)
            {
                yield return new ValidationResult("Start time of auction can't be in past", start);
            }
            if (StartTime > DateTime.Now.AddDays(1))
            {
                yield return new ValidationResult("Start time of auction can't be more then 1 day", start);
            }

        }
    }

    public class LotEditModel:LotModel
    {
        [DisplayName("Price")]
        [GreaterNullValidation]
        [PriceValidation]
        public override double Price { get; set; }

        [DisplayName("Lot Id")]
        public override int Id { get; set; }

        [DisplayName("Name")]
        public override string Name { get; set; }

        [DisplayName("Description")]
        public override string Description { get; set; }
        
        [DisplayName("Picture")]
        public override string Picture { get; set; }

        [DisplayName("Start time of auction for this lot")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public override DateTime StartTime { get; set; }

        [DisplayName("End time of auction for this lot")]
        public override DateTime DateEnd { get; set; }

        [DisplayName("Customer")]
        public override int Customer_id { get; set; }

        [DisplayName("Owner")]
        public override int Owner_id { get; set; }

        [DisplayName("Category")]
        public override int Category_id { get; set; }

        public LotEditModel()
        { }
        public LotEditModel(Lot lot):base()
        {
            Description = lot.Description;
            Price = lot.Price;
            Picture = lot.Picture;
            Name = lot.Name;
            Id = lot.Id;
            Customer_id = lot.Customer_id;
            Owner_id = lot.Owner_id;
            StartTime = lot.StartTime;
            DateEnd = lot.DateEnd;
            Category_id = lot.Category_id;
        }
    }

    public class LotModel // : IValidatableObject //,IComparable<Lot>,Comparer<Lot>
    {

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual double Price { get; set; }
        public virtual string Picture { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime DateEnd { get; set; }
        public virtual int Customer_id { get; set; }
        public virtual int Owner_id { get; set; }
        public virtual int Category_id { get; set; }
    }
}
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mvc_auction.Models
{
    public class Lot // : IValidatableObject //,IComparable<Lot>,Comparer<Lot>
    {
        [DisplayName("Lot Id")]
        public virtual int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Please, enter the name of Lot")]
        public virtual string Name { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }

        [DisplayName("Price")]
        [Required]
        [GreaterNullValidation]
        public virtual double Price { get; set; }

        [DisplayName("Picture")]
        public virtual string Picture { get; set; }

        [DisplayName("Start time of auction for this lot")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public virtual DateTime StartTime { get; set; }

        [DisplayName("End time of auction for this lot")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public virtual DateTime DateEnd { get; set; }

        [DisplayName("Customer")]
        public virtual int Customer_id { get; set; }

        [DisplayName("Owner")]
        public virtual int Owner_id { get; set; }

        [DisplayName("Category")]
        public virtual int Category_id { get; set; }
        
        //public virtual int Id { get; set; }
        //public virtual string Name { get; set; }
        //public virtual string Description { get; set; }
        //public virtual double Price { get; set; }
        //public virtual string Picture { get; set; }
        //public virtual DateTime StartTime { get; set; }
        //public virtual DateTime DateEnd { get; set; }
        //public virtual int Customer_id { get; set; }
        //public virtual int Owner_id { get; set; }
        //public virtual int Category_id { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    // поля для которых реализована custom validation
        //    var end = new[] { "DateEnd" };
        //    var start = new[] { "StartTime" };

        //    if (DateEnd <= DateTime.Now)
        //    {
        //        yield return new ValidationResult("Date end of auction can't be in past", end);
        //    }
        //    if (DateEnd > DateTime.Now.AddDays(7))
        //    {
        //        yield return new ValidationResult("Date end of auction can't be more then week", end);
        //    }

        //    if (StartTime < DateTime.Now)
        //    {
        //        yield return new ValidationResult("Start time of auction can't be in past", start);
        //    }
        //    if (StartTime > DateTime.Now.AddDays(1))
        //    {
        //        yield return new ValidationResult("Start time of auction can't be more then 1 day", start);
        //    }

        //}


        //public int CompareTo(Lot other)
        //{
        //    if (other == null) return 1;
        //    return DateEnd.CompareTo(other.DateEnd);
        //}

        //public override int Compare(Lot x, Lot y)
        //{
        //    if (x.DateEnd.CompareTo(y.DateEnd)!=0)
        //    {
        //        return x.DateEnd.CompareTo(y.DateEnd);
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
    }
}
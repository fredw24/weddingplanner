using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
                    // auto-implemented properties need to match the columns in your table
            // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
            [Key]
            public int WeddingId { get; set; }
            // MySQL VARCHAR and TEXT types can be represeted by a string
            [Required]
            [Display(Name = "Wedding One:")]
            public string WeddingOne { get; set; }
            [Required]
            [Display(Name = "Wedding Two:")]
            public string WeddingTwo { get; set; }

            [Required]
            [Time]
            [Display(Name = "Date Time:")]
            public DateTime Date { get; set; }
            [Required]
            [Display(Name = "Address:")]
            public string WeddingAddress {get; set;}

            public DateTime CreatedAt {get; set;} = DateTime.Now;
            public DateTime UpdatedAt{get;set;} = DateTime.Now;

            public int UserId {get;set;}

            public List<Guest> Guest {get;set;}

       
    }

        public class TimeAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                DateTime plan = Convert.ToDateTime(value); 
                if (plan <= DateTime.Now)
                    return new ValidationResult("the Date is too Late!");
                return ValidationResult.Success;
            }
        }

}
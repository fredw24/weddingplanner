using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Guest
    {
                    // auto-implemented properties need to match the columns in your table
            // the [Key] attribute is used to mark the Model property being used for your table's Primary Key
            [Key]
            public int GuestId { get; set; }
            // MySQL VARCHAR and TEXT types can be represeted by a string
            public int UserId {get; set;}

            public int WeddingId {get; set;}

            public Users Users {get; set;}
            public Wedding Wedding {get; set;}

       
    }
}
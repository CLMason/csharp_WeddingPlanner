using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get; set;}

        [Required(ErrorMessage="Wedder One name is required")]
        [MinLength(2, ErrorMessage="Name must be 2 characters or longer")]
        public string NameOne {get;set;}

        [Required(ErrorMessage="Wedder Two name is required")]
        [MinLength(2, ErrorMessage="Name must be 2 characters or longer")]
        public string NameTwo {get;set;}

        [Required(ErrorMessage="Wedding date is required")]
        [DataType(DataType.Date,ErrorMessage="Date cannot be in the past.")]
        [FutureDate]
        
        public DateTime Date {get;set;}

        [Required(ErrorMessage ="Wedding Address is required")]
        public string Address {get;set;}

        public int UserId {get;set;}

        public List<Guest> Attendees {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    
    }
}
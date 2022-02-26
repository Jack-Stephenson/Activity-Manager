using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace belt.Models
{
    public class Hangout
    {
        [Key]
        public int HangoutId {get;set;}
        [Required]
        [Display(Name ="What we're doing")]
        public string Pastime {get;set;}
        [Display(Name ="Duration")]
        public int? Duration {get;set;}
        public string Unit {get;set;}

        [Display(Name ="Date of Pastime")]
        [DataType(DataType.DateTime)]
        public DateTime DateTime {get;set;}
        [Required]
        public int CreatorId {get;set;}
        [Required]
        [Display(Name ="Description")]
        public string Description {get;set;}
        public List<Attendance> Participants {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}
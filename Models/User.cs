using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace belt.Models
{
    public class PasswordAttribute : ValidationAttribute
    {
        public string GetErrorMessage() =>
            "Password must be at lease 8 characters, and must contain at least 1 number, 1 letter, and 1 special character";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (((string)value) == null)
            {
                return new ValidationResult(GetErrorMessage());
            }
            bool letter = ((string)value).Any(c=>char.IsLetter(c));
            bool number = ((string)value).Any(c=>char.IsNumber(c));
            bool special = ((string)value).Any(c=>!char.IsLetterOrDigit(c));
            bool length = false;
            if (((string)value).Length > 7)
            {
                length = true;
            }
            if (letter == false || number == false || special == false || length == false)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }
    }
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "First name")]
        [MinLength(2)]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [MinLength(2)]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Password")]
        [Password]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public List<Attendance> Hangouts { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
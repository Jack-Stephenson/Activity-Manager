using System.ComponentModel.DataAnnotations;

namespace belt.Models
{
    public class LoginUser
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Must be a valid email address")]
        public string LoginEmail {get;set;}
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string LoginPassword {get;set;}
    }
}
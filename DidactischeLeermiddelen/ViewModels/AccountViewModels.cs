using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DidactischeLeermiddelen.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [Display(Name = "Herinner mijn login?")]
        public bool RememberMe { get; set; }
    }

}

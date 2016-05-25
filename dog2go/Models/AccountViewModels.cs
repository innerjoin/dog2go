using System.ComponentModel.DataAnnotations;
using dog2go.Backend.Model;

namespace dog2go.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You are required to provide a nickname"), StringLength(20, MinimumLength = 3, ErrorMessage = "Must be between 3 and 20 characters")]
        [Display(Name = "User name")]
        [RegularExpression(@"^[a-zäöüA-Z0-9_\-''-'\s]*$" ,ErrorMessage = "Invalid character inserted!")]
        public string UserName { get; set; }
    }

    public class DisplayNameModel
    {
        public int TableIdentifier;
        public User DisplayUser;
        public string ColorMeeplePath;
    }
}

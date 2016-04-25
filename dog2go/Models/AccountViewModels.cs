using System.ComponentModel.DataAnnotations;

namespace dog2go.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You are required to provide a nickname"), StringLength(20, MinimumLength = 3, ErrorMessage = "Must be between 3 and 20 characters")]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace dog2go.Backend.Model
{
    public class User
    {

        public User() { }
        public string Identifier { get; set; }

        [Required(ErrorMessage = "You are required to provide a nickname"), StringLength(20, MinimumLength = 3,  ErrorMessage = "Must be between 3 and 20 characters")]
        public string Nickname { get; set; }
        public string GroupName { get; set; }
        public string Cookie { get; set; }
        public User(string name)
        {
            Nickname = name;
        }
        public User(string name, string connectionId)
        {
            Nickname = name;
            Identifier = connectionId;
        }
    }
}

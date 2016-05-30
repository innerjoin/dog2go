using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dog2go.Backend.Model
{
    public class User
    {

        public User() { }
        public string Identifier { get; set; }
        public string Nickname { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
        public bool CardDropped;
        public User(string name)
        {
            Nickname = name;
        }
        public User(string name, string connectionId)
        {
            Nickname = name;
            Identifier = connectionId;
            CardDropped = false;
        }
    }
}

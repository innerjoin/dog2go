using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dog2go.Models
{
    public class ChooseTableViewModels
    {
        [Required(ErrorMessage = "No GameTable exists")]
        //[Display(Name = "User name")]
        public List<int> GameTableIdList { get; set; }
        public int GameTableId { get; set; }

    }
}

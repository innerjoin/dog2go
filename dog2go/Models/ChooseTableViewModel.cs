using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace dog2go.Models
{
    public class ChooseTableViewModel
    {
        public ChooseTableViewModel()
        {
            GameTableList = new List<TableViewModel>();
        }
        public List<TableViewModel> GameTableList { get; set; }
    }

    public class TableViewModel
    {
        public TableViewModel(int identifier, string name)
        {
            this.Identifier = identifier;
            this.Name = name;
        }
        public TableViewModel(){}
        public int Identifier { get; set; }

        [Required(ErrorMessage = "You are required to provide a tablename"), StringLength(20, MinimumLength = 3, ErrorMessage = "Must be between 3 and 20 characters")]
        public string Name { get; set; }
    }
}

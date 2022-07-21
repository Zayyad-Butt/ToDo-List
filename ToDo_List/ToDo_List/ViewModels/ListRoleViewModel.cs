using System.ComponentModel.DataAnnotations;

namespace ToDo_List.ViewModels
{

    public class ListRoleViewModel
    {
        public string Id { get; set; }
        [Required]
        public string RoleName { get; set; }
    }


}

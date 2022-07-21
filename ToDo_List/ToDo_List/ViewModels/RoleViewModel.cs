using System.ComponentModel.DataAnnotations;

namespace ToDo_List.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required]
        public string RoleName { get; set; }

    }
}

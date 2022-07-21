using System.ComponentModel.DataAnnotations;

namespace ToDo_List.ViewModels
{
    public class TaskList
    {
        public int ID { get; set; }
        [Required]
        [StringLength(30)]
        public string TaskName { get; set; }

        [Required]
        public string Description { get; set; }
    }
}

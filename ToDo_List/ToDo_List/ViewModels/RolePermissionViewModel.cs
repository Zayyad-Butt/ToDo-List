namespace ToDo_List.ViewModels
{
    public class RolePermissionViewModel
    {
        public RolePermissionViewModel()
        {
            Claims = new List<RoleClaim>();
        }
        public string RoleId { get; set; }
        public List<RoleClaim> Claims { get; set; }
       


    }
}

using System.Security.Claims;

namespace ToDo_List.Models
{
    public class PermissionStore
    {
        public static List<Claim> AllPermissions = new List<Claim>()
        {
            new Claim("Create List","Create List"),
            new Claim("Edit List","Edit List"),
            new Claim("Delete List", "Delete List"),
            new Claim("Display List", "Display List")
        };
    }
}

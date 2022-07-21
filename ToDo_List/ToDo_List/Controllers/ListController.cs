using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ToDo_List.ViewModels;

namespace ToDo_List.Controllers
{
    [Authorize]
    public class ListController : Controller
    {
        [HttpGet]
        [Authorize (Policy= "CreateListPolicy")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize (Policy= "CreateListPolicy")]
        public IActionResult Index(TaskList model)
        {
            if (ModelState.IsValid)
            {
                string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ToDoList;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                SqlConnection con = new SqlConnection(connString);
                
                string name = model.TaskName;
                string desc = model.Description;
                string query = "insert into TaskDetail" +
                    "(name,descrription) values(@n,@d)";

                SqlParameter p1 = new SqlParameter("n", name);
                SqlParameter p2 = new SqlParameter("d", desc);

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return RedirectToAction("Display", "List"); 
            }
            return View();
        }

        [HttpGet]
        [Authorize (Policy= "EditListPolicy")]
        public IActionResult Update(string taskId)
        {
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ToDoList;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(connString);

            con.Open();

            string query = "select * from TaskDetail where Id=@id";
            SqlParameter p4 = new SqlParameter("id", taskId);

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p4);
            SqlDataReader dr = cmd.ExecuteReader();
            TaskList tl = new TaskList();
            dr.Read();
            tl.ID = (int)dr[0];
            tl.TaskName = dr[1].ToString();
            tl.Description = dr[2].ToString();
            
            con.Close();

            return View(tl);
        }        
        [HttpPost]
        [Authorize (Policy= "EditListPolicy")]
        public IActionResult Update(TaskList model)
        {
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ToDoList;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(connString);

            con.Open();

            string query = "update TaskDetail set name=@n, descrription=@d where Id=@id";
            SqlParameter p1 = new SqlParameter("n", model.TaskName);
            SqlParameter p2 = new SqlParameter("d", model.Description);
            SqlParameter p3 = new SqlParameter("id", model.ID);

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p1);
            cmd.Parameters.Add(p2);
            cmd.Parameters.Add(p3);
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Display","List");
        }
        [HttpGet]
        [Authorize (Policy= "DeleteListPolicy")]
        public IActionResult Delete(string taskId)
        {
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ToDoList;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(connString);

            con.Open();

            string query = "DELETE FROM TaskDetail WHERE Id=@id";
            SqlParameter p3 = new SqlParameter("id", taskId);

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.Add(p3);
            cmd.ExecuteNonQuery();
            con.Close();
            return RedirectToAction("Display", "List");
        }
        [HttpGet]
        [Authorize (Policy= "DisplayListPolicy")]
        public IActionResult Display()
        {
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ToDoList;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection con = new SqlConnection(connString);

            con.Open();
            string query = "Select * from TaskDetail";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<TaskList> l = new List<TaskList>();
            while (dr.Read())
            {
                TaskList tl=new TaskList();
                tl.ID = (int)dr[0];
                tl.TaskName = dr[1].ToString();
                tl.Description = dr[2].ToString();
                l.Add(tl);
            }
            con.Close();
            return View(l);
        }
    }
}

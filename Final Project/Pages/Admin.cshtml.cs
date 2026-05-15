using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.Data.SqlClient;
using Pesach_Project.Model;

namespace Pesach_Project.Pages
{
    public class WatchUsersModel : PageModel
    {
        public DataTable dt { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Admin") != "True")
            {
                return RedirectToPage("/Index");
            }
            Helper helper = new Helper();
            string SQL = "SELECT * FROM Users";
            dt  = helper.RetrieveTable(SQL, "Users");
            return Page();
        }
    }
}

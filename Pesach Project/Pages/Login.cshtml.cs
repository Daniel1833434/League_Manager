using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class LoginModel : PageModel
    {
        public string msg { get; set; } = string.Empty;
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            string SQLStr = $"SELECT * FROM Users WHERE UserName LIKE '{Username}' AND Password LIKE '{Password}'";
            Helper helper = new Helper();
            DataTable dt = helper.RetrieveTable(SQLStr, "Users");

            if (dt.Rows.Count > 0)
            {
                HttpContext.Session.SetString("UserName", Username);
                HttpContext.Session.SetString("UserId", dt.Rows[0]["Id"].ToString());
                HttpContext.Session.SetString("Admin", dt.Rows[0]["Admin"].ToString());
                return RedirectToPage("/Index");
            }
            msg = "Wrong username or password";
            return Page();
        }
    }
}

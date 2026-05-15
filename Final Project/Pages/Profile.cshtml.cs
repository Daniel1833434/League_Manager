using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class ProfileModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; } = new User();
        public string msg { get; set; } = string.Empty;
        public string Id { get; set; }
        public IActionResult OnGet()
        {
            Id = HttpContext.Session.GetString("UserId");
            Helper helper = new Helper();
            string SQL = $"SELECT * FROM Users WHERE Id = {Id}";
            DataTable dt = helper.RetrieveTable(SQL, "Users");
            DataRow row = dt.Rows[0];
            NewUser.UserName = row["UserName"].ToString();
            NewUser.FirstName = row["FirstName"].ToString();
            NewUser.LastName = row["LastName"].ToString();
            NewUser.Email = row["Email"].ToString();
            NewUser.PhoneNum = row["PhoneNum"].ToString();
            NewUser.BirthDate = DateTime.Parse(row["BirthDate"].ToString());
            NewUser.Admin = bool.Parse(row["Admin"].ToString());
            
            return Page();
        }
        public IActionResult OnPost()
        {
            Helper helper = new Helper();
            Id = HttpContext.Session.GetString("UserId");
            NewUser.Id = int.Parse(Id);
            NewUser.Admin = bool.Parse(HttpContext.Session.GetString("Admin"));
            int n = helper.UpdateUsers(NewUser, "Users");
            if (n == -1)
            {
                msg = "Username already taken.";
                return Page();
            }
            HttpContext.Session.SetString("UserName", NewUser.UserName);
            return RedirectToPage("/Index");
        }
    }
}

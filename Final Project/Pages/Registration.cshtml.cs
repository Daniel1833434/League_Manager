using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Pesach_Project.Model;

namespace Pesach_Project.Pages
{
    public class RegistrationModel : PageModel
    {
        [BindProperty]
        public User NewUser { get; set; }
        public string msg { get; set; } = string.Empty;

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            Helper helper = new Helper();
            int n = helper.InsertToUsers(NewUser, "Users");
            if (n == -1)
            {
                msg = "Username already taken.";
                return Page();
            }
            HttpContext.Session.SetString("UserName", NewUser.UserName);
            HttpContext.Session.SetString("UserId", NewUser.Id.ToString()); 
            HttpContext.Session.SetString("Admin", NewUser.Admin.ToString()); 
            return RedirectToPage("/Index");
        }
    }
}


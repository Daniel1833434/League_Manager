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
            int n = helper.Insert(NewUser, "UsersPesach");
            if (n == -1)
            {
                msg = "Username already taken.";
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}


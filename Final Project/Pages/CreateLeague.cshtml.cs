using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;

namespace Pesach_Project.Pages
{
    public class CreateLeagueModel : PageModel
    {
        [BindProperty]
        public League NewLeague { get; set; }
        public string msg { get; set; } = string.Empty;
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            Helper helper = new Helper();
            NewLeague.OwnerId = int.Parse(HttpContext.Session.GetString("UserId"));
            int n = helper.InsertToLeagues(NewLeague, "Leagues");
            if (n == -1)
            {
                msg = "League name already taken.";
                return Page();
            }

            return RedirectToPage("/Index");
        }
    }
}

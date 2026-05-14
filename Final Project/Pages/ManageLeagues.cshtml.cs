using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;

namespace Pesach_Project.Pages
{
    public class ManageLeaguesModel : PageModel
    {
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string msg { get; set; } = string.Empty;

        [BindProperty]
        public string NewName { get; set; }
        public void OnGet()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            LeagueName = HttpContext.Session.GetString("LeagueName");
        }
        public IActionResult OnPostChangeName()
        {
            Helper helper = new Helper();
            int n = helper.UpdateLeagueName(int.Parse(HttpContext.Session.GetString("LeagueId")), int.Parse(HttpContext.Session.GetString("UserId")), NewName);
            if (n == -1)
            {
                msg = "League name already taken.";
                return Page();
            }
            HttpContext.Session.SetString("LeagueName", NewName);
            return RedirectToPage();
        }
    }
}

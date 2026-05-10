using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pesach_Project.Pages
{
    public class ManageLeaguesModel : PageModel
    {
        public string LeagueId { get; set; }
        public string LeagueName { get; set; }
        public void OnGet()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            LeagueName = HttpContext.Session.GetString("LeagueName");
        }
    }
}

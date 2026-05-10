using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class ManagePlayersModel : PageModel
    {
        [BindProperty]
        public Player NewPlayer { get; set; }
        public string LeagueId { get; set; }
        public DataTable dt { get; set; }
        public string msg { get; set; } = string.Empty;
        public IActionResult OnGet()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            Helper helper = new Helper();
            string SQL = $"SELECT PlayerName FROM Players WHERE LeagueId = {LeagueId}";
            dt = helper.RetrieveTable(SQL, "Leagues");
            return Page();
        }
        public IActionResult OnPost()
        {
            Helper helper = new Helper();
            NewPlayer.LeagueId = int.Parse(HttpContext.Session.GetString("LeagueId"));
            int n = helper.InsertToPlayers(NewPlayer, "Players");
            if (n == -1)
            {
                msg = "Player name already taken.";
            }
            return RedirectToPage();
        }
    }
}

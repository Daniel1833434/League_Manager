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

        [BindProperty]
        public string PlayerId { get; set; }
        [BindProperty]
        public string NewName { get; set; }
        public string LeagueId { get; set; }
        public DataTable dt { get; set; }
        public string msg { get; set; } = string.Empty;
        public void LoadPlayers()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            Helper helper = new Helper();
            string SQL = $"SELECT Id, PlayerName FROM Players WHERE LeagueId = {LeagueId}";
            dt = helper.RetrieveTable(SQL, "Players");
        }
        public IActionResult OnGet()
        { 
            LoadPlayers();
            return Page();
        }
        public IActionResult OnPostAdd()
        {
            Helper helper = new Helper();
            NewPlayer.LeagueId = int.Parse(HttpContext.Session.GetString("LeagueId"));
            int n = helper.InsertToPlayers(NewPlayer, "Players");
            if (n == -1)
            {
                msg = "Player name already taken.";
                LoadPlayers();
                return Page();
            }
            return RedirectToPage();
        }
        public IActionResult OnPostDelete()
        {
            Helper helper = new Helper();
            helper.DeleteRow(PlayerId, "Players");
            return RedirectToPage("/ManagePlayers");

        }
        public IActionResult OnPostChangeName()
        {
            Helper helper = new Helper();
            int n = helper.UpdatePlayerName(int.Parse(HttpContext.Session.GetString("LeagueId")), PlayerId, NewName);
            if(n == -1)
            {
                msg = "Player name already taken.";
                LoadPlayers();
                return Page();
            }
           return RedirectToPage();
        }
    }
}

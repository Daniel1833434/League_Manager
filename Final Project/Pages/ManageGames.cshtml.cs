using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class ManageGamesModel : PageModel
    {
        [BindProperty]
        public Game newGame { get; set; }

        [BindProperty]
        public string gameId { get; set; }
        public DataTable playersDt { get; set; }

        public string UserName { get; set; }
        public string Id { get; set; }
        public DataTable dt { get; set; }
        public string LeagueId { get; set; }
        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString("UserName");
            Id = HttpContext.Session.GetString("UserId");
            LeagueId = HttpContext.Session.GetString("LeagueId");
            Helper helper = new Helper();

            string playersSQL = $"SELECT Id, PlayerName FROM Players WHERE LeagueId = {LeagueId}";
            playersDt = helper.RetrieveTable(playersSQL, "Players");

            string SQL = $"SELECT Id,Player1Name, Player1Score, Player2Name, Player2Score FROM Games WHERE LeagueId = {int.Parse(LeagueId)}";
            dt = helper.RetrieveTable(SQL, "Games");
            return Page();
        }
        public IActionResult OnPostAddGame()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            newGame.LeagueId = int.Parse(LeagueId);
            Helper helper = new Helper();
            helper.InsertToGames(newGame, "Games");
            return RedirectToPage();
        }
        public IActionResult OnPostDelete()
        {
            Helper helper = new Helper();
            helper.DeleteRow(gameId, "Games");
            return RedirectToPage("/ManageGames");

        }
        public IActionResult OnPostUpdate()
        {
            HttpContext.Session.SetString("GameId", gameId);
            return RedirectToPage("/UpdateGame");
        }

        public IActionResult OnPostGenerateGames()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            Helper helper = new Helper();
            helper.DeleteAllGamesFromLeague(int.Parse(LeagueId));
            helper.GenerateGamesToLeague(int.Parse(LeagueId));
            return RedirectToPage("/ManageGames");
        }
    }
}

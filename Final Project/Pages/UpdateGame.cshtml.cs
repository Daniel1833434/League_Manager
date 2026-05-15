using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class UpdateGameModel : PageModel
    {
        public string gameId { get; set; }
        [BindProperty]
        public Game newGame { get; set; } = new Game();
        public DataTable playersDt { get; set; }
        public string LeagueId { get; set; }
        public IActionResult OnGet()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            gameId = HttpContext.Session.GetString("GameId");
            Helper helper = new Helper();

            string playersSQL = $"SELECT Id, PlayerName FROM Players WHERE LeagueId = {LeagueId}";
            playersDt = helper.RetrieveTable(playersSQL, "Players");

            string SQL = $"SELECT * FROM Games WHERE Id = {gameId}";
            DataTable dt = helper.RetrieveTable(SQL, "Games");
            DataRow row = dt.Rows[0];

            newGame.Player1Id = int.Parse(row["Player1Id"].ToString());
            newGame.Player1Name = row["Player1Name"].ToString();
            newGame.Player1Score = int.Parse(row["Player1Score"].ToString());
            newGame.Player2Id = int.Parse(row["Player2Id"].ToString());
            newGame.Player2Name = row["Player2Name"].ToString();
            newGame.Player2Score = int.Parse(row["Player2Score"].ToString());

            return Page();
        }
        public IActionResult OnPostUpdate()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            gameId = HttpContext.Session.GetString("GameId");
            newGame.LeagueId = int.Parse(LeagueId);
            Helper helper = new Helper();
            helper.UpdateGame(newGame, gameId);
            return RedirectToPage("/ManageGames");
        }
    }
}

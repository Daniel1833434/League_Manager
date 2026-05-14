using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class UpdateGameModel : PageModel
    {
        public string gameId { get; set; }
        public Game lastGame { get; set; } = new Game();
        [BindProperty]
        public Game newGame { get; set; }
        public DataTable playersDt { get; set; }
        public string LeagueId { get; set; }
        public void OnGet()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            gameId = HttpContext.Session.GetString("GameId");
            Helper helper = new Helper();

            string playersSQL = $"SELECT Id, PlayerName FROM Players WHERE LeagueId = {LeagueId}";
            playersDt = helper.RetrieveTable(playersSQL, "Players");

            string SQL = $"SELECT * FROM Games WHERE Id = {gameId}";
            DataTable dt = helper.RetrieveTable(SQL, "Games");
            DataRow row = dt.Rows[0];

            lastGame.Player1Id = int.Parse(row["Player1Id"].ToString());
            lastGame.Player1Name = row["Player1Name"].ToString();
            lastGame.Player1Score = int.Parse(row["Player1Score"].ToString());
            lastGame.Player2Id = int.Parse(row["Player2Id"].ToString());
            lastGame.Player2Name = row["Player2Name"].ToString();
            lastGame.Player2Score = int.Parse(row["Player2Score"].ToString());
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

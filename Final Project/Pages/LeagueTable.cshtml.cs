using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pesach_Project.Model;
using System.Data;

namespace Pesach_Project.Pages
{
    public class LeagueTableModel : PageModel
    {
        public string LeagueId { get; set; }
        public string medal { get; set; }
        public DataTable dt { get; set; }
        public int rank { get; set; } = 1;
        public void OnGet()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            Helper helper = new Helper();
            helper.UpdateAllPlayersStatsFromLeague(int.Parse(LeagueId));
            string SQL = $"SELECT PlayerName, PlayerPoints, MatchesPlayed FROM Players WHERE LeagueId = {LeagueId} ORDER BY PlayerPoints DESC";
            dt = helper.RetrieveTable(SQL, "Players");
        }
    }
}

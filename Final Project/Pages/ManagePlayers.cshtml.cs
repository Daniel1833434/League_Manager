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
        [BindProperty]
        public string MaxPlayers { get; set; }

        public int CurrentPlayers { get; set; }

        public void UpdateCurrentPlayers()
        {
            CurrentPlayers = 0;
            foreach (DataRow row in dt.Rows)
            {
                CurrentPlayers += 1;
            }
        }
        public void LoadPlayers()
        {
            LeagueId = HttpContext.Session.GetString("LeagueId");
            Helper helper = new Helper();
            string SQL = $"SELECT Id, PlayerName FROM Players WHERE LeagueId = {LeagueId}";
            dt = helper.RetrieveTable(SQL, "Players");
            
            string SQL2 = $"SELECT MaxPlayers FROM Leagues WHERE Id = {LeagueId}";
            DataTable dt2 = helper.RetrieveTable(SQL2, "Leagues");
            MaxPlayers = dt2.Rows[0]["MaxPlayers"].ToString();
        }
        public IActionResult OnGet()
        { 
            LoadPlayers();
            UpdateCurrentPlayers();
            return Page();
        }
        public IActionResult OnPostAdd()
        {
            LoadPlayers();
            Helper helper = new Helper();
            NewPlayer.LeagueId = int.Parse(HttpContext.Session.GetString("LeagueId"));
            UpdateCurrentPlayers();
            if (CurrentPlayers >= int.Parse(MaxPlayers))
            {
                msg = "League is at max capacity.";
                return Page();
            }
            int n = helper.InsertToPlayers(NewPlayer, "Players");
            if (n == -1)
            {
                msg = "Player name already taken.";
                return Page();
            }
            UpdateCurrentPlayers();
            return RedirectToPage();
        }
        public IActionResult OnPostDelete()
        {
            Helper helper = new Helper();
            LoadPlayers();
            helper.DeletePlayer(PlayerId, "Players");
            UpdateCurrentPlayers();
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
        public IActionResult OnPostChangeMaxPlayers()
        {
            Helper helper = new Helper();
            LeagueId = HttpContext.Session.GetString("LeagueId");
            helper.UpdateLeagueMaxPlayers(int.Parse(LeagueId), int.Parse(MaxPlayers));
            return RedirectToPage("/ManagePlayers");
        }
        
    }
}

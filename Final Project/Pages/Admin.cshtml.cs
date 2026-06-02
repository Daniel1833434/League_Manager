using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Microsoft.Data.SqlClient;
using Pesach_Project.Model;

namespace Pesach_Project.Pages
{
    public class WatchUsersModel : PageModel
    {
        public DataTable Usersdt { get; set; }
        public DataTable Leaguesdt { get; set; }
        public DataTable Playersdt { get; set; }
        public DataTable Gamessdt { get; set; }
        [BindProperty]
        public string PlayerId { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public string gameId { get; set; }
        [BindProperty]
        public string LeagueId { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Admin") != "True")
            {
                return RedirectToPage("/Index");
            }
            Helper helper = new Helper();
            string SQL = "SELECT * FROM Users Where Admin = 'False'";
            Usersdt  = helper.RetrieveTable(SQL, "Users");

            SQL = "SELECT * FROM Leagues";
            Leaguesdt = helper.RetrieveTable(SQL, "Leagues");

            SQL = "SELECT * FROM Players";
            Playersdt = helper.RetrieveTable(SQL, "Players");

            SQL = "SELECT * FROM Games";
            Gamessdt = helper.RetrieveTable(SQL, "Games");

            return Page();
        }
        public IActionResult OnPostUpdateUser()
        {
            Helper helper = new Helper();
            HttpContext.Session.SetString("UserIdUpdate", UserId);
            return RedirectToPage("/Profile", new { AdminChanging = true });
        }
        public IActionResult OnPostDeleteUser()
        {
            Helper helper = new Helper();
            helper.DeleteRow(UserId, "Users");
            return RedirectToPage("/Admin");
        }
        public IActionResult OnPostMakeAdmin()
        {
            Helper helper = new Helper();
            helper.MakeAdmin(int.Parse(UserId), "Users");
            return RedirectToPage("/Admin");
        }
        public IActionResult OnPostDeleteLeague()
        {
            Helper helper = new Helper();
            helper.DeleteRow(LeagueId, "Leagues");
            return RedirectToPage("/Admin");
        }
        public IActionResult OnPostDeletePlayer()
        {
            Helper helper = new Helper();
            helper.DeletePlayer(PlayerId, "Players");
            return RedirectToPage("/Admin");

        }
        public IActionResult OnPostDeleteGame()
        {
            Helper helper = new Helper();
            helper.DeleteRow(gameId, "Games");
            return RedirectToPage("/Admin");
        }
    }
}

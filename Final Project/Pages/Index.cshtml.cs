using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using Pesach_Project.Model;

namespace Pesach_Project.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string UserName { get; set; }
        
        [BindProperty]
        public string LeagueId { get; set; }
        [BindProperty]
        public string LeagueName { get; set; }
        public string Id { get; set; }
        public DataTable dt { get; set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            UserName = HttpContext.Session.GetString("UserName");
            Id = HttpContext.Session.GetString("UserId");
            if (Id != null)
            {
                Helper helper = new Helper();
                string SQL = $"SELECT Id ,LeagueName, MaxPlayers FROM Leagues WHERE OwnerId = {Id}";
                dt = helper.RetrieveTable(SQL, "Leagues"); 
            }
            return Page();
        }
        public IActionResult OnPostDelete()
        {
            Helper helper = new Helper();
            helper.DeleteRow(LeagueId, "Leagues");
            return RedirectToPage("/Index");
            
        }
        public IActionResult OnPostManage()
        {
            HttpContext.Session.SetString("LeagueName", LeagueName);
            HttpContext.Session.SetString("LeagueId", LeagueId);
            return RedirectToPage("/ManageLeagues");
        }
    }
}

using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Pesach_Project.Pages
{
    public class SportsLeaguesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public List<FootballStanding> FootballTable { get; set; } = new List<FootballStanding>();

        public SportsLeaguesModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            await LoadFootballTable();
        }

        private async Task LoadFootballTable()
        {
            string apiKey = _configuration["ApiSports:Key"];

            HttpClient client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("x-apisports-key", apiKey);

            string url = "https://v3.football.api-sports.io/standings?league=39&season=2024";

            string json = await client.GetStringAsync(url);

            JsonDocument document = JsonDocument.Parse(json);

            var standings = document
                .RootElement
                .GetProperty("response")[0]
                .GetProperty("league")
                .GetProperty("standings")[0];

            foreach (var row in standings.EnumerateArray())
            {
                FootballTable.Add(new FootballStanding
                {
                    Rank = row.GetProperty("rank").GetInt32(),
                    TeamName = row.GetProperty("team").GetProperty("name").GetString(),
                    Played = row.GetProperty("all").GetProperty("played").GetInt32(),
                    Wins = row.GetProperty("all").GetProperty("win").GetInt32(),
                    Draws = row.GetProperty("all").GetProperty("draw").GetInt32(),
                    Losses = row.GetProperty("all").GetProperty("lose").GetInt32(),
                    GoalsFor = row.GetProperty("all").GetProperty("goals").GetProperty("for").GetInt32(),
                    GoalsAgainst = row.GetProperty("all").GetProperty("goals").GetProperty("against").GetInt32(),
                    GoalDifference = row.GetProperty("goalsDiff").GetInt32(),
                    Points = row.GetProperty("points").GetInt32()
                });
            }
        }

        public class FootballStanding
        {
            public int Rank { get; set; }
            public string TeamName { get; set; }
            public int Played { get; set; }
            public int Wins { get; set; }
            public int Draws { get; set; }
            public int Losses { get; set; }
            public int GoalsFor { get; set; }
            public int GoalsAgainst { get; set; }
            public int GoalDifference { get; set; }
            public int Points { get; set; }
        }

      
    }
}
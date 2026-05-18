using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Pesach_Project.Pages
{
    public class SportsLeaguesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public List<FootballStanding> FootballTable { get; set; } = new List<FootballStanding>();
        public List<BasketballStanding> BasketballTable { get; set; } = new List<BasketballStanding>();

        public SportsLeaguesModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            await LoadFootballTable();
            await LoadBasketballTable();
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

        private async Task LoadBasketballTable()
        {
            string apiKey = _configuration["ApiSports:Key"];

            HttpClient client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-apisports-key", apiKey);

            string url = "https://v1.basketball.api-sports.io/standings?league=12&season=2024-2025X";

            string json = await client.GetStringAsync(url);

            JsonDocument document = JsonDocument.Parse(json);

            JsonElement responseArray = document
                .RootElement
                .GetProperty("response");

            List<BasketballStanding> tempTable = new List<BasketballStanding>();
            HashSet<int> addedTeams = new HashSet<int>();

            foreach (JsonElement group in responseArray.EnumerateArray())
            {
                if (group.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement row in group.EnumerateArray())
                    {
                        AddBasketballRow(row, tempTable, addedTeams);
                    }
                }
                else if (group.ValueKind == JsonValueKind.Object)
                {
                    AddBasketballRow(group, tempTable, addedTeams);
                }
            }

            BasketballTable = tempTable
                .OrderByDescending(team => team.Wins)
                .ThenBy(team => team.Losses)
                .ThenByDescending(team => team.PointsFor - team.PointsAgainst)
                .ToList();

            for (int i = 0; i < BasketballTable.Count; i++)
            {
                BasketballTable[i].Rank = i + 1;
            }
        }

        private void AddBasketballRow(JsonElement row, List<BasketballStanding> tempTable, HashSet<int> addedTeams)
        {
            int teamId = row.GetProperty("team").GetProperty("id").GetInt32();

            if (addedTeams.Contains(teamId))
            {
                return;
            }

            addedTeams.Add(teamId);

            int pointsFor = row.GetProperty("points").GetProperty("for").GetInt32();
            int pointsAgainst = row.GetProperty("points").GetProperty("against").GetInt32();

            tempTable.Add(new BasketballStanding
            {
                TeamName = row.GetProperty("team").GetProperty("name").GetString(),
                Played = row.GetProperty("games").GetProperty("played").GetInt32(),
                Wins = row.GetProperty("games").GetProperty("win").GetProperty("total").GetInt32(),
                Losses = row.GetProperty("games").GetProperty("lose").GetProperty("total").GetInt32(),
                PointsFor = pointsFor,
                PointsAgainst = pointsAgainst
            });
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

        public class BasketballStanding
        {
            public int Rank { get; set; }
            public string TeamName { get; set; }
            public int Played { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int PointsFor { get; set; }
            public int PointsAgainst { get; set; }
        }
    }
}
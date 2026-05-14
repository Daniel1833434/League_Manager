namespace Pesach_Project.Model
{
    public class Game
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public string Player1Name { get; set; } = string.Empty;
        public int Player1Score { get; set; }
        public int Player2Id { get; set; }
        public string Player2Name { get; set; } = string.Empty;
        public int Player2Score { get; set; }
        public int LeagueId { get; set; }
    }
}

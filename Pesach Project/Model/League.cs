namespace Pesach_Project.Model
{
    public class League
    {
        public int Id { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public int OwnerId { get; set; }
    }
}

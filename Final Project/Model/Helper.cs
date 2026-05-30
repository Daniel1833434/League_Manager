using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Numerics;

namespace Pesach_Project.Model
{
    public class Helper
    {
        private string conString;

        public Helper()
        {
            var configuration  = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            conString = configuration.GetConnectionString("DB");
        }
        public DataTable RetrieveTable(string SQLStr , string table)
        {
            SqlConnection con = new SqlConnection(conString);

            SqlCommand cmd = new SqlCommand(SQLStr, con);

            SqlDataAdapter ad = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();

            ad.Fill(ds, table);

            return ds.Tables[table];
        }

        public int InsertToUsers(User user, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM {table} WHERE UserName Like '{user.UserName}'";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            if (ds.Tables[table].Rows.Count > 0)
            {
                return -1;
            }

            // בניית השורה להוספה
            DataRow dr = ds.Tables[table].NewRow();
            dr["UserName"] = user.UserName;
            dr["FirstName"] = user.FirstName;
            dr["LastName"] = user.LastName;
            dr["Password"] = user.Password;
            dr["Email"] = user.Email;
            dr["PhoneNum"] = user.PhoneNum;
            dr["BirthDate"] = user.BirthDate;
            dr["Admin"] = false;

            ds.Tables[table].Rows.Add(dr);

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, table);
            return n;
        }

        public int InsertToLeagues(League league, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM {table} WHERE OwnerID = {league.OwnerId} AND LeagueName = '{league.LeagueName}'";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            if (ds.Tables[table].Rows.Count > 0)
            {
                return -1;
            }

            // בניית השורה להוספה
            DataRow dr = ds.Tables[table].NewRow();
            dr["LeagueName"] = league.LeagueName;
            dr["MaxPlayers"] = league.MaxPlayers;
            dr["OwnerId"] = league.OwnerId;

            ds.Tables[table].Rows.Add(dr);

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, table);
            return n;
        }
        public int DeleteRow(string Id, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // שולפים רק את השורה שרוצים למחוק
            string SQLStr = $"SELECT * FROM {table} WHERE Id = {Id}";

            SqlCommand cmd = new SqlCommand(SQLStr, con);
            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            // אם לא נמצאה שורה עם ה-Id הזה
            if (ds.Tables[table].Rows.Count == 0)
            {
                return 0;
            }

            // מוחקים את השורה מתוך ה-DataSet
            ds.Tables[table].Rows[0].Delete();

            // עדכון הדאטה בייס
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

            int n = adapter.Update(ds, table);

            return n;
        }
        public int DeletePlayer(string Id, string table)
        {
            SqlConnection con = new SqlConnection(conString);

            DataSet ds = new DataSet();

            // 1. Delete all games with this player
            string gamesSQL = $"SELECT * FROM Games WHERE Player1Id = {int.Parse(Id)} OR Player2Id = {int.Parse(Id)}";

            SqlCommand gamesCmd = new SqlCommand(gamesSQL, con);

            SqlDataAdapter gamesAdapter = new SqlDataAdapter(gamesCmd);
            gamesAdapter.Fill(ds, "Games");

            foreach (DataRow row in ds.Tables["Games"].Rows)
            {
                row.Delete();
            }

            SqlCommandBuilder gamesBuilder = new SqlCommandBuilder(gamesAdapter);
            int deletedGames = gamesAdapter.Update(ds, "Games");


            // 2. Delete the player himself
            string playerSQL = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand playerCmd = new SqlCommand(playerSQL, con);
            playerCmd.Parameters.AddWithValue("@Id", Id);

            SqlDataAdapter playerAdapter = new SqlDataAdapter(playerCmd);
            playerAdapter.Fill(ds, table);

            if (ds.Tables[table].Rows.Count == 0)
            {
                return 0;
            }

            ds.Tables[table].Rows[0].Delete();

            SqlCommandBuilder playerBuilder = new SqlCommandBuilder(playerAdapter);
            int deletedPlayers = playerAdapter.Update(ds, table);

            return deletedPlayers;
        }
        public int InsertToPlayers(Player player, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM {table} WHERE LeagueId = {player.LeagueId} AND PlayerName LIKE '{player.PlayerName}'";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            if (ds.Tables[table].Rows.Count > 0)
            {
                return -1;
            }

            // בניית השורה להוספה
            DataRow dr = ds.Tables[table].NewRow();
            dr["PlayerName"] = player.PlayerName;
            dr["LeagueId"] = player.LeagueId;

            ds.Tables[table].Rows.Add(dr);

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, table);
            return n;
        }

        public int UpdatePlayerName(int LeagueId,string playerId, string newName)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players WHERE LeagueId = {LeagueId} AND PlayerName LIKE '{newName}'";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            if (ds.Tables["Players"].Rows.Count > 0)
            {
                return -1;
            }

            SQLStr = $"SELECT * FROM Players WHERE LeagueId = {LeagueId} AND Id = {playerId}";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            DataRow dr = ds.Tables["Players"].Select($"Id = {playerId}")[0];
            UpdateGamesPlayerName(newName, dr["PlayerName"].ToString(), LeagueId.ToString());
            dr["PlayerName"] = newName;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
           
            int n = adapter.Update(ds, "Players");
            return n;
        }
        public void UpdateGamesPlayerName(string NewPlayerName, string OldPlayerName, string LeagueId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Games WHERE LeagueId = {LeagueId} AND (Player1Name = '{OldPlayerName}' OR Player2Name = '{OldPlayerName}')";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Games");

            cmd = new SqlCommand(SQLStr, con);

            foreach (DataRow dr in ds.Tables["Games"].Rows)
            {
                if (dr["Player1Name"].ToString() == OldPlayerName)
                {
                    dr["Player1Name"] = NewPlayerName;
                }
                if (dr["Player2Name"].ToString() == OldPlayerName)
                {
                    dr["Player2Name"] = NewPlayerName;
                }
            }

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Games");
        }
        public int UpdateLeagueName(int LeagueId,int OwnerId ,string newName)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Leagues WHERE OwnerId = {OwnerId} AND LeagueName LIKE '{newName}'";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Leagues");

            if (ds.Tables["Leagues"].Rows.Count > 0)
            {
                return -1;
            }

            SQLStr = $"SELECT * FROM Leagues ";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Leagues");

            DataRow dr = ds.Tables["Leagues"].Select($"Id = {LeagueId}")[0];
            dr["LeagueName"] = newName;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, "Leagues");
            return n;
        }

        public int UpdateLeagueMaxPlayers(int LeagueId, int newMaxPlayers)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Leagues";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Leagues");

            SQLStr = $"SELECT * FROM Leagues ";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Leagues");

            DataRow dr = ds.Tables["Leagues"].Select($"Id = {LeagueId}")[0];
            dr["MaxPlayers"] = newMaxPlayers;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, "Leagues");
            return n;
        }

        public void InsertToGames(Game game, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Games";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            // בניית השורה להוספה
            DataRow dr = ds.Tables[table].NewRow();
            dr["Player1Id"] = game.Player1Id;
            dr["Player1Name"] = game.Player1Name;
            dr["Player1Score"] = game.Player1Score;
            dr["Player2Id"] = game.Player2Id;
            dr["Player2Name"] = game.Player2Name;
            dr["Player2Score"] = game.Player2Score;
            dr["LeagueId"] = game.LeagueId;

            ds.Tables[table].Rows.Add(dr);

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, table);
        }

        public void AddPlayerPoints(int PlayerId, int Points)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players ";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            SQLStr = $"SELECT * FROM Players WHERE Id = {PlayerId}";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            DataRow dr = ds.Tables["Players"].Select($"Id = {PlayerId}")[0];
            dr["PlayerPoints"] = (int)dr["PlayerPoints"] + Points;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Players");
        }

        public void ZeroPlayerPoints(int PlayerId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players ";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            SQLStr = $"SELECT * FROM Players WHERE Id = {PlayerId}";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            DataRow dr = ds.Tables["Players"].Select($"Id = {PlayerId}")[0];
            dr["PlayerPoints"] = 0;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Players");
        }
        public void AddPlayerGame(int PlayerId,int Games)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players ";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            SQLStr = $"SELECT * FROM Players WHERE Id = {PlayerId}";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            DataRow dr = ds.Tables["Players"].Select($"Id = {PlayerId}")[0];
            dr["MatchesPlayed"] = (int)dr["MatchesPlayed"] + Games;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Players");
        }

        public void ZeroPlayerGame(int PlayerId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players ";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            SQLStr = $"SELECT * FROM Players WHERE Id = {PlayerId}";
            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            DataRow dr = ds.Tables["Players"].Select($"Id = {PlayerId}")[0];
            dr["MatchesPlayed"] = 0;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Players");
        }
        public void UpdateGame(Game game, string gameId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Games";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Games");

            cmd = new SqlCommand(SQLStr, con);

            adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Games");

            DataRow dr = ds.Tables["Games"].Select($"Id = {gameId}")[0];

            dr["Player1Id"] = game.Player1Id;
            dr["Player1Name"] = game.Player1Name;
            dr["Player1Score"] = game.Player1Score;
            dr["Player2Id"] = game.Player2Id;
            dr["Player2Name"] = game.Player2Name;
            dr["Player2Score"] = game.Player2Score;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            adapter.Update(ds, "Games");
        }

        public void UpdatePlayerStats(int PlayerId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);
            ////// checking points and matches in games
            ///// בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Games WHERE Player1Id = {PlayerId} OR Player2Id = {PlayerId}";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Games");

            ZeroPlayerPoints(PlayerId);
            ZeroPlayerGame(PlayerId);

            foreach (DataRow row in ds.Tables["Games"].Rows)
            {
                if ((int)row["Player1Id"] == PlayerId)
                {
                    if((int)row["Player1Score"] > (int)row["Player2Score"])
                    {
                        AddPlayerPoints(PlayerId,3);
                    }
                }
                else
                {

                    if ((int)row["Player2Score"] > (int)row["Player1Score"])
                    {
                        AddPlayerPoints(PlayerId, 3);
                    }
                }

                if ((int)row["Player1Score"] == (int)row["Player2Score"])
                {
                    AddPlayerPoints(PlayerId, 1);
                }
                AddPlayerGame(PlayerId, 1);
            }
        }
        public void UpdateAllPlayersStatsFromLeague(int leagueId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);
            ///// בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players WHERE LeagueId = {leagueId}";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            foreach (DataRow row in ds.Tables["Players"].Rows)
            {
                UpdatePlayerStats((int)row["Id"]);
            }

        }
        public int UpdateUsers(User user, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM {table}";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            // בניית השורה להוספה
            DataRow dr = ds.Tables[table].Select($"Id = {user.Id}")[0];
            ;
            dr["UserName"] = user.UserName;
            dr["FirstName"] = user.FirstName;
            dr["LastName"] = user.LastName;
            dr["Password"] = user.Password;
            dr["Email"] = user.Email;
            dr["PhoneNum"] = user.PhoneNum;
            dr["BirthDate"] = user.BirthDate;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, table);
            return n; 
        }
        public int MakeAdmin(int userId, string table)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM {table}";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, table);

            // בניית השורה להוספה
            DataRow dr = ds.Tables[table].Select($"Id = {userId}")[0];
            ;
            dr["Admin"] = true;

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, table);
            return n;
        }
        public int DeleteAllGamesFromLeague(int leagueId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Games WHERE LeagueId = {leagueId}";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Games");

            // בניית השורה להוספה
            foreach (DataRow dr in ds.Tables["Games"].Rows)
            {
                dr.Delete();
            }

            // עדכון הדאטה סט בבסיס הנתונים
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            int n = adapter.Update(ds, "Games");
            return n;
        }
        public void GenerateGamesToLeague(int leagueId)
        {
            // התחברות למסד הנתונים
            SqlConnection con = new SqlConnection(conString);

            // בניית פקודת SQL
            string SQLStr = $"SELECT * FROM Players WHERE LeagueId = {leagueId}";
            SqlCommand cmd = new SqlCommand(SQLStr, con);

            // בניית DataSet
            DataSet ds = new DataSet();

            // טעינת סכימת הנתונים
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds, "Players");

            int playersCount = ds.Tables["Players"].Rows.Count;
            // בניית השורה להוספה
            for (int distance = 1; distance < playersCount; distance++)
            {
               for(int i = 0; i + distance< playersCount; i++)
               {
                    int j = i + distance;
                    Game game = new Game();
                    game.Player1Id = (int)ds.Tables["Players"].Rows[i]["Id"];
                    game.Player2Id = (int)ds.Tables["Players"].Rows[j]["Id"];
                    game.Player1Name = ds.Tables["Players"].Rows[i]["PlayerName"].ToString();
                    game.Player2Name = ds.Tables["Players"].Rows[j]["PlayerName"].ToString();
                    game.Player1Score = 0;
                    game.Player2Score = 0;
                    game.LeagueId = leagueId;
                    InsertToGames(game, "Games");
                }
            }
        }
    }
}

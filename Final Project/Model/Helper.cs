using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

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
            dr["Password"] = user.Password;
            dr["Email"] = user.Email;
            dr["PhoneNum"] = user.PhoneNum;
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
            string SQLStr = $"SELECT * FROM {table} WHERE LeagueName Like '{league.LeagueName}'";
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
            string SQLStr = $"SELECT * FROM {table} WHERE Id = @Id";

            SqlCommand cmd = new SqlCommand(SQLStr, con);
            cmd.Parameters.AddWithValue("@Id", Id);

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
    }
}

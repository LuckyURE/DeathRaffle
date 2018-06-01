using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using Repository.HelperModels;

namespace Repository.Repositories
{
    public class AppInfoRepository
    {
        private readonly string _connectionString;

        public AppInfoRepository()
        {
            var connectString = new SqliteConnectionStringBuilder
            {
                DataSource = RepositorySettings.DatabaseFile
            };

            _connectionString = connectString.ToString();
        }
        
        private IDbConnection Connection => new SqliteConnection(_connectionString);

        public void UpdateLumensRequired(decimal lumensRequired)
        {
            using (var dbConnection = Connection)
            {
                const string query =
                    @"UPDATE AppInfo
                      SET LumensRequired = @LumensRequired";
                
                dbConnection.Open();
                dbConnection.Execute(query, new {LumensRequired = lumensRequired});
            }
        }

        public AppInfo GetAppInfo()
        {
            using (var dbConnection = Connection)
            {
                const string query = 
                    @"SELECT w.Address, a.LumensRequired
                      FROM Wallets w, AppInfo a";
                
                dbConnection.Open();
                return dbConnection.Query<AppInfo>(query).First();
            }
        }
    }
}
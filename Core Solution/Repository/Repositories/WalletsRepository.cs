using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using Repository.Models;

namespace Repository.Repositories
{
    public class WalletsRepository
    {
        private readonly string _connectionString;

        public WalletsRepository()
        {
            var connectString = new SqliteConnectionStringBuilder
            {
                DataSource = RepositorySettings.DatabaseFile
            };

            _connectionString = connectString.ToString();
        }

        private IDbConnection Connection => new SqliteConnection(_connectionString);
        
        public Wallet Add(Wallet wallet)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                var id = dbConnection.Insert(wallet);

                wallet.Id = id;
                return wallet;
            }
        }

        public Wallet GetWallet(string accountId)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                const string query = @"SELECT *
                                       FROM Wallets
                                       WHERE Address = @Address";
                try
                {
                    return dbConnection.Query<Wallet>(query, new {Address = accountId}).FirstOrDefault();
                }
                catch
                {
                    return null;
                }
            }
        }

        public void SaveLastToken(Wallet wallet)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Update(wallet);
            }
        }
    }
}
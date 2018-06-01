using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using Repository.Models;

namespace Repository.Repositories
{
    public class TicketRepository
    {
        private readonly string _connectionString;

        public TicketRepository()
        {
            var connectString = new SqliteConnectionStringBuilder
            {
                DataSource = RepositorySettings.DatabaseFile
            };

            _connectionString = connectString.ToString();
        }

        private IDbConnection Connection => new SqliteConnection(_connectionString);
        
        public long Add(Ticket ticket)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Insert(ticket);
            }
        }

        public IEnumerable<Ticket> GetAll()
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Ticket>("SELECT * FROM Tickets");
            }
        }

        public Ticket GetById(long id)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Get<Ticket>(id);
            }
        }
        
        public void Update(Ticket ticket)
        {
            using (var dbConnection = Connection)
            {
                const string query = @"UPDATE Tickets SET 
                                            PlayerAddress = @PlayerAddress,
                                            CelebrityId = @CelebrityId,
                                            PoolId = @PoolId
                                       WHERE Id = @Id";
                dbConnection.Open();
                dbConnection.Query(query, ticket);
            }
        }
        
        public void Delete(int id)
        {
            using (var dbConnection = Connection)
            {
                const string query = @"DELETE FROM Tickets
                                       WHERE Id = @Id";
                dbConnection.Open();
                dbConnection.Execute(query, new { Id = id });
            }
        }
    }
}
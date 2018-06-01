using System.Collections.Generic;
using System.Data;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using Repository.HelperModels;
using Repository.Models;

namespace Repository.Repositories
{
    public class PoolRepository
    {
        private readonly string _connectionString;

        public PoolRepository()
        {
            var connectString = new SqliteConnectionStringBuilder
            {
                DataSource = RepositorySettings.DatabaseFile
            };

            _connectionString = connectString.ToString();
        }

        private IDbConnection Connection => new SqliteConnection(_connectionString);
        
        public long Add(Pool pool)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Insert(pool);
            }
        }

        public IEnumerable<Pool> GetAll()
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.GetAll<Pool>();
            }
        }

        public Pool GetById(int id)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Get<Pool>(id);
            }
        }
        
        public void Update(Pool pool)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Update(pool);
            }
        }
        
        public void Delete(Pool pool)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Delete(pool);
            }
        }

        public long GetAvailablePoolByPayer(string playerAddress)
        {
            using (var dbConnection = Connection)
            {
                const string query = 
                    @"SELECT pls.Id AS PoolId, Count(tiks.PlayerAddress) AS TicketCount
                      FROM Pools pls
	                  INNER JOIN Tickets tiks ON pls.Id = tiks.PoolId
                      WHERE LockDate IS NULL AND pls.Id NOT IN (SELECT pl.Id 
				                                                FROM Pools pl
					                                            INNER JOIN Tickets ti ON pl.Id = ti.PoolId
				                                                WHERE ti.PlayerAddress = @PlayerAddress)
                      GROUP BY pls.Id
                      ORDER BY TicketCount DESC
                      LIMIT 1";
                
                dbConnection.Open();
                var result = dbConnection
                    .QuerySingleOrDefault<AvailablePoolByPlayer>(query, new {PlayerAddress = playerAddress});

                return result?.PoolId ?? 0;
            }
        }

        public long GetAvailablePoolCelebrity(long poolId)
        {
            using (var dbConnection = Connection)
            {
                const string query = @"
                    SELECT c.Id
                    FROM Celebrities c
                    WHERE c.Id NOT IN (SELECT t.CelebrityId
								       FROM Tickets t
								       WHERE t.PoolId = @PoolId)
                    AND c.Dead = 0
                    ORDER BY RANDOM()
                    LIMIT 1";
                
                dbConnection.Open();
                var result =  dbConnection
                    .QuerySingleOrDefault<AvailablePoolCelebrity>(query, new { PoolId = poolId });

                return result?.Id ?? 0;
            }
        }
        
        public IEnumerable<AdminDashboardPools> GetAdminDashboardPools(bool active)
        {
            var queryMod = active ? "IS NULL" : "IS NOT NULL";
            
            using (var dbConnection = Connection)
            {
                var query = 
                    $@"SELECT p.Id AS Id, Count(t.PlayerAddress) AS TicketCount,
                             p.CreateDate AS CreateDate, p.LockDate AS GameStarted, p.CloseDate AS GameEnded
                      FROM Pools p
	                  INNER JOIN Tickets t ON p.Id = t.PoolId
                      WHERE CloseDate {queryMod}
                      GROUP BY p.Id
                      ORDER BY TicketCount DESC";
                
                dbConnection.Open();
                var results = dbConnection.Query<AdminDashboardPools>(query);
                return results;
            }
        }
        
        public IEnumerable<DashboardPool> GetDashboardPools()
        {
            using (var dbConnection = Connection)
            {
                const string query = 
                    @"SELECT p.Id AS PoolId, Count(t.PlayerAddress) AS TicketCount,
                             p.CreateDate AS CreateDate, p.LockDate AS GameStarted
                      FROM Pools p
	                  INNER JOIN Tickets t ON p.Id = t.PoolId
                      WHERE CloseDate IS NULL
                      GROUP BY p.Id
                      ORDER BY TicketCount DESC";
                
                dbConnection.Open();
                var results = dbConnection.Query<DashboardPool>(query);
                return results;
            }
        }
    }
}
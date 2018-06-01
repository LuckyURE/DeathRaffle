using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using Repository.Models;

namespace Repository.Repositories
{
    public class CelebrityRepository
    {
        private readonly string _connectionString;

        public CelebrityRepository()
        {
            var connectString = new SqliteConnectionStringBuilder
            {
                DataSource = RepositorySettings.DatabaseFile
            };

            _connectionString = connectString.ToString();
        }

        private IDbConnection Connection => new SqliteConnection(_connectionString);

        public Celebrity Add(Celebrity celebrity)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                var id = dbConnection.Insert(celebrity);

                celebrity.Id = id;
                return celebrity;
            }
        }

        public IEnumerable<Celebrity> GetAll()
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Celebrity>("SELECT * FROM Celebrities");
            }
        }

        public IEnumerable<Celebrity> GetLiving()
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Celebrity>(
                    @"  SELECT * 
                        FROM Celebrities c
                        WHERE c.Dead = 0");
            }
        }

        public Celebrity GetById(long id)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Get<Celebrity>(id);
            }
        }
        
        public Celebrity GetByLastNameAndBirthYear(string lastName, int birthYear)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();

                const string query = @"
                    SELECT *
                    FROM Celebrities c
                    WHERE c.LastName = @LastName AND
                          c.BirthYear = @BirthYear
                ";

                return dbConnection.QueryFirstOrDefault<Celebrity>(
                    query, new {LastName = lastName, BirthYear = birthYear});
            }
        }

        public void Update(Celebrity celebrity)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Update(celebrity);
            }
        }

        public IEnumerable<string> MarkDead(Celebrity celebrity)
        {
            celebrity.Dead = true;
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                var winnersToBePaid = new List<string>();
                using (var transaction = dbConnection.BeginTransaction())
                {
                    try
                    {
                        // Steps:
                        // 1) Set celeb as dead.
                        // 2) Get all tickets with that celebrity.
                        // 3) Get all locked pools with that ticket.
                        // 4) Set the winner for the pools.
                        // 5) Set the close date for the pools.
                        // 6) Pay the winning tickets for those pools.
                        // 7) Maybe send a payment to the others to say they lost in the future?
                        // 8) Now find all tickets with that celebrity in pools not locked.
                        // 9) Reassign a new celebrity to that ticket and send a payment with an update.

                        const string markDeadSql =
                            @" UPDATE Celebrities
                           SET Dead = 1
                           WHERE Id = @Id
                        ";

                        dbConnection.Execute(markDeadSql, new {celebrity.Id});

                        const string allTicketsSql =
                            @"  SELECT *
                            FROM Tickets t
                            WHERE t.CelebrityId = @Id
                        ";

                        var allTickets = dbConnection.Query<Ticket>(allTicketsSql, new {celebrity.Id}).ToList();
                        var poolIds = allTickets.Select(p => int.Parse(p.PoolId.ToString())).ToArray();

                        const string allPoolsSql =
                            @"  SELECT *
                            FROM Pools p
                            WHERE p.Id IN @poolIds
                        ";

                        var allPools = dbConnection
                            .Query<Pool>(
                                allPoolsSql,
                                new {poolIds})
                            .ToList();

                        var lockedPools = allPools.FindAll(p => p.LockDate != null);

                        foreach (var pool in lockedPools)
                        {
                            var ticket = allTickets.Single(p => p.PoolId == pool.Id && p.CelebrityId == celebrity.Id);

                            const string setWinnerSql =
                                @"  UPDATE Pools
                                SET WinningTicket = @TicketId, CloseDate = date('now')
                                WHERE Id = @PoolId
                            ";

                            dbConnection.Execute(setWinnerSql, new {TicketId = ticket.Id, PoolId = pool.Id});
                            winnersToBePaid.Add(ticket.PlayerAddress);
                        }

                        var notLockedPools = allPools.Where(p => p.LockDate == null).ToList();

                        foreach (var pool in notLockedPools)
                        {
                            var ticketId = allTickets
                                .Single(p => p.PoolId == pool.Id && p.CelebrityId == celebrity.Id)
                                .Id;

                            var poolRepository = new PoolRepository();
                            var newCelebrityId = poolRepository.GetAvailablePoolCelebrity(pool.Id);

                            const string newCelebSql =
                                @"  UPDATE Tickets
                                SET CelebrityId = @CelebId
                                WHERE Id = @TicketId
                            ";

                            dbConnection.Execute(newCelebSql, new {CelebId = newCelebrityId, TicketId = ticketId});
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }

                    return winnersToBePaid;
                }
            }
        }

        public void Delete(Celebrity celebrity)
        {
            using (var dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Delete(celebrity);
            }
        }
    }
}
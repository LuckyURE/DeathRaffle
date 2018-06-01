using System.IO;
using Microsoft.Data.Sqlite;

namespace Repository
{
    public static class DatabaseUtils
    {
        public static void ConfigureDatabase()
        {
            var dataDir = Path.Combine("/var/www/DeathRaffle/Data");

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            var dbFile = Path.Combine(dataDir, "DeathRaffle.sqlite");
            RepositorySettings.DatabaseFile = dbFile;

            if (File.Exists(dbFile)) return;

            var conn = new SqliteConnection($"Data Source={dbFile};");
            conn.Open();

            var command = conn.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=WAL;";
            command.ExecuteNonQuery();

            command.CommandText = @"
                    CREATE TABLE Celebrities (
                        Id INTEGER PRIMARY KEY,
                        FirstName    TEXT    NOT NULL,
                        MiddleName   TEXT,
                        LastName     TEXT    NOT NULL,
                        Title        TEXT,
                        Suffix       TEXT,
                        BirthYear    INTEGER NOT NULL,
                        Description  TEXT    NOT NULL,
                        Dead         BOOLEAN NOT NULL,
                        Country      TEXT    NOT NULL
                    );
                    
                    CREATE TABLE Pools (
                        Id        INTEGER PRIMARY KEY,
                        WinningTicket INTEGER,
                        CreateDate    TEXT    NOT NULL,
                        CloseDate     TEXT,
                        LockDate      TEXT
                    );

                    CREATE TABLE Tickets (
                        Id       INTEGER PRIMARY KEY,
                        PlayerAddress TEXT    NOT NULL,
                        CelebrityId    INTEGER NOT NULL
                                               REFERENCES Celebrities (Id) ON DELETE NO ACTION
                                                                                     ON UPDATE NO ACTION,
                        PoolId         INTEGER NOT NULL
                                               REFERENCES Pools (Id) ON DELETE NO ACTION
                                                                          ON UPDATE NO ACTION
                    );

                    CREATE TABLE Wallets (
                        Id        INTEGER PRIMARY KEY AUTOINCREMENT,
                        Address    TEXT    NOT NULL
                                            UNIQUE,
                        LastToken    TEXT
                    );

                    CREATE TABLE AppInfo (
                        Id                INTEGER PRIMARY KEY AUTOINCREMENT,
                        LumensRequired    INTEGER NOT NULL
                    );
                ";

            command.ExecuteNonQuery();
        }
    }
}
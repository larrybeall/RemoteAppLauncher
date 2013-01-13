using Dapper;
using DapperExtensions;
using RemoteAppLauncher.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace RemoteAppLauncher.Data.Repositories
{
    internal class PersistedItemRepository : BaseRepository<PersistedFileItem>
    {
        private const string CreateTable =
            "CREATE TABLE PersistedFileItem (Id TEXT NOT NULL, Name TEXT,Path TEXT,Pinned INTEGER,Accesses INTEGER,PRIMARY KEY (Id))";

        private readonly string _dbPath;
        private readonly string _cnString;

        public PersistedItemRepository()
        {
            _dbPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _cnString = string.Format("Data Source={0};Version=3;", _dbPath);
            Init();
        }

        public IEnumerable<PersistedFileItem> GetPinnedItems(SQLiteConnection connection)
        {
            return Execute((cn) =>
                {
                    var predicate = Predicates.Field<PersistedFileItem>(f => f.Pinned, Operator.Eq, true);
                    return cn.GetList<PersistedFileItem>(predicate);
                }, connection);
        }

        protected override string GetConnectionString()
        {
            return _cnString;
        }

        private void Init()
        {
            if(File.Exists(_dbPath))
                return;

            SQLiteConnection.CreateFile(_dbPath);
            Execute((cn) =>
                {
                    cn.Execute(CreateTable);
                });
        }
    }
}

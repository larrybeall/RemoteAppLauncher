﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlServerCe;
using RemoteAppLauncher.Properties;

namespace RemoteAppLauncher.Data
{
    internal class DataConfigManager
    {
        private const string CreatePersistedItemTable =
            "CREATE TABLE PersistedFileItem (Id TEXT NOT NULL, Name TEXT, Path TEXT, Directory TEXT, Pinned INTEGER, Accesses INTEGER, PRIMARY KEY (Id))";

        private static volatile DataConfigManager _instance;
        private static object _syncRoot = new Object();

        private readonly string _dbPath;
        private readonly string _cnString;

        private DataConfigManager()
        {
            _dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "launcher.sdf");
            _cnString = string.Format("Data Source={0};", _dbPath);
            Init();
        }

        public static DataConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if(_instance == null)
                            _instance = new DataConfigManager();
                    }
                }

                return _instance;
            }
        }

        public string ConnectionString
        {
            get { return _cnString; }
        }

        private void Init()
        {
            if (File.Exists(_dbPath))
                return;

            using (SqlCeEngine engine = new SqlCeEngine(_cnString))
            {
                engine.CreateDatabase();
            }

            using (var cn = new SqlCeConnection(_cnString))
            {
                cn.Open();
                cn.Execute(Resources.PersistedItemTableCreate);
                cn.Execute(Resources.PersistedItemCreateConstraint);
                cn.Execute(Resources.PersistedItemCreateIndex);
                cn.Close();
            }
        }
    }
}

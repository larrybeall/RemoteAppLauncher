using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace RemoteAppLauncher.Data.Repositories
{
    internal abstract class BaseRepository<TEntity>
        where TEntity : class, new()
    {
        protected virtual string GetConnectionString()
        {
            // intentionally left invalid to require derived class to implement.
            return null;
        }

        public SQLiteConnection GetConnection()
        {
            string cnString = GetConnectionString();
            if(string.IsNullOrEmpty(cnString))
                throw new InvalidOperationException("GetConnectionString is not implemented or did not return your connection string.");

            return new SQLiteConnection(cnString);
        }

        public TEntity Get(string id, SQLiteConnection connection = null)
        {
            return Execute(cn => cn.Get<TEntity>(id), connection);
        }

        public IEnumerable<TEntity> GetAll(SQLiteConnection connection = null)
        {
            return Execute(cn => cn.GetList<TEntity>(), connection);
        }

        public void Insert(TEntity item, SQLiteConnection connection = null)
        {
            Execute(cn => cn.Insert(item), connection);
        }

        public void Insert(IEnumerable<TEntity> item, SQLiteConnection connection = null)
        {
            Execute(cn => cn.Insert(item), connection);
        }

        public void Remove(string id, SQLiteConnection connection = null)
        {
            Execute(cn =>
            {
                var entity = cn.Get<TEntity>(id);
                if (entity == null)
                    return;

                cn.Delete(entity);
            }, connection);
        }

        public void Update(TEntity item)
        {

        }

        protected void Execute(Action<SQLiteConnection> toExecute, SQLiteConnection connection = null)
        {
            Execute<Object>(cn =>
            {
                toExecute(cn);
                return null;
            }, connection);
        }

        protected TReturn Execute<TReturn>(Func<SQLiteConnection, TReturn> toExecute, SQLiteConnection connection = null)
        {
            bool shouldManageConnection = connection == null;
            SQLiteConnection cn = null;
            TReturn result;

            try
            {
                if (shouldManageConnection)
                {
                    cn = GetConnection();
                    cn.Open();
                }

                result = toExecute(cn);
            }
            finally
            {
                if (shouldManageConnection && cn != null)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }

            return result;
        }
    }
}

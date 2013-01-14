using System.IO;
using Dapper;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;

namespace RemoteAppLauncher.Data.Repositories
{
    internal abstract class BaseRepository<TEntity>
        where TEntity : class, new()
    {
        public SqlCeConnection GetConnection()
        {
            return new SqlCeConnection(DataConfigManager.Instance.ConnectionString);
        }

        public TEntity Get(string id, SqlCeConnection connection = null)
        {
            return Execute(cn => cn.Get<TEntity>(id), connection);
        }

        public IEnumerable<TEntity> GetAll(SqlCeConnection connection = null)
        {
            return Execute(cn => cn.GetList<TEntity>(), connection);
        }

        public void Insert(TEntity item, SqlCeConnection connection = null)
        {
            Execute(cn => cn.Insert(item), connection);
        }

        public void Insert(IEnumerable<TEntity> item, SqlCeConnection connection = null)
        {
            Execute(cn => cn.Insert(item), connection);
        }

        public void Remove(string id, SqlCeConnection connection = null)
        {
            Execute(cn =>
            {
                var entity = cn.Get<TEntity>(id);
                if (entity == null)
                    return;

                Remove(entity, cn);
            }, connection);
        }

        public void Remove(TEntity entity, SqlCeConnection connection = null)
        {
            Execute(cn => cn.Delete(entity), connection);
        }

        public void Remove(IEnumerable<TEntity> entities, SqlCeConnection connection = null)
        {
            Execute(cn =>
                {
                    foreach (var entity in entities)
                    {
                        cn.Delete(entity);
                    }
                }, connection);
        }

        public void Update(TEntity item, SqlCeConnection connection = null)
        {
            Execute(cn => cn.Update<TEntity>(item), connection);
        }

        protected void Execute(Action<SqlCeConnection> toExecute, SqlCeConnection connection = null)
        {
            Execute<Object>(cn =>
            {
                toExecute(cn);
                return null;
            }, connection);
        }

        protected TReturn Execute<TReturn>(Func<SqlCeConnection, TReturn> toExecute, SqlCeConnection connection = null)
        {
            bool shouldManageConnection = connection == null;
            SqlCeConnection cn = connection;
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

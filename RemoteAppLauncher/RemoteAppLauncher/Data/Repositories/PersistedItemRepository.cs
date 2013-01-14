using System.Data.SqlServerCe;
using Dapper;
using DapperExtensions;
using RemoteAppLauncher.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RemoteAppLauncher.Data.Repositories
{
    internal class PersistedItemRepository : BaseRepository<PersistedFileItem>
    {
        public IEnumerable<PersistedFileItem> GetPinnedItems(SqlCeConnection connection)
        {
            return Execute((cn) =>
                {
                    var predicate = Predicates.Field<PersistedFileItem>(f => f.Pinned, Operator.Eq, true);
                    return cn.GetList<PersistedFileItem>(predicate);
                }, connection);
        }
    }
}

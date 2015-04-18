using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angular_webapi_base.Services.Interfaces
{
    public interface ISQLDataContext : IDisposable
    {
        IEnumerable<IDictionary<string, object>> Get(string sql);
        IEnumerable<IDictionary<string, object>> Get(string sql, IDictionary<string, object> data);

        IDictionary<string, object> Set(string sql, IDictionary<string, object> data);

        IEnumerable<IDictionary<string, object>> Query(string sql);

        int BulkLoad(string table, IEnumerable<string[]> rawData, IDictionary<string, Type> columnMap);

        IEnumerable<IDictionary<string, object>> TableValueParameter(string sql, IEnumerable<string[]> rawData,
            IDictionary<string, Type> columnMap, string parameter);

        void Transaction();
        bool Rollback();

    }
}

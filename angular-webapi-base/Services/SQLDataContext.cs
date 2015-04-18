using System.Data;
using System.Data.SqlTypes;
using System.Xml;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using angular_webapi_base.Services.Interfaces;

namespace angular_webapi_base.Services
{
    /// <summary>
    /// Object for querying sql data
    /// </summary>
    public class SQLDataContext : ISQLDataContext
    {
        protected SqlConnection _connection { get; set; }
        protected SqlTransaction _transaction { get; set; }
        protected bool _canRollBack { get; set; }

        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLDataContext(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _canRollBack = false;
        }

        /// <summary>
        /// cleanup everything
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
            {
                if (_transaction != null && _canRollBack)   //only commit if there is a transaction and we havent rolledback.
                    _transaction.Commit();

                _connection.Dispose();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// set this to be transactional
        /// </summary>
        public void Transaction()
        {
            _transaction = _connection.BeginTransaction();
            _canRollBack = true;
        }

        /// <summary>
        /// rollback our transaction
        /// </summary>
        /// <returns></returns>
        public bool Rollback()
        {
            if (_transaction == null || !_canRollBack)
                return false;

            _transaction.Rollback();
            _canRollBack = false;

            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// run a sql query and returns the result set. used for getting data ( generally rows )
        /// </summary>
        /// <param name="sql">the stored proc to call</param>
        /// <returns>all rows returned from the stored proc</returns>
        public IEnumerable<IDictionary<string, object>> Get(string sql)
        {
            return RunQuery(sql, CommandType.StoredProcedure);
        }

        public IEnumerable<IDictionary<string, object>> Get(string sql, IDictionary<string, object> data)
        {
            return RunQuery(sql, CommandType.StoredProcedure, data);
        }



        /// <summary>
        /// runs a sql query and returns the result.  used for setting data
        /// </summary>
        /// <param name="sql">the stored proc to call</param>
        /// <param name="data">the data in dictionary format</param>
        /// <returns>the first row or null returned from the stored proc</returns>
        public IDictionary<string, object> Set(string sql, IDictionary<string, object> data)
        {
            return RunQuery(sql, CommandType.StoredProcedure, data).FirstOrDefault();
        }

        public IEnumerable<IDictionary<string, object>> Query(string sql)
        {
            return RunQuery(sql, CommandType.Text);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// run a sql query
        /// </summary>
        /// <param name="sql">the stored proc</param>
        /// <param name="data">data for the stored proc parameters</param>
        /// <returns>all rows returned by the stored proc</returns>

        protected IEnumerable<IDictionary<string, object>> RunQuery(string sql, CommandType cmdType,
            IDictionary<string, object> data = null)
        {
            var result = new List<IDictionary<string, object>>();

            using (var comm = new SqlCommand(sql, _connection))
            {
                comm.CommandType = cmdType;

                data.Each(param =>
                {
                    if (param.Value !=null && param.Value.GetType() == typeof (XDocument))
                    {
                        var xml = new SqlParameter(param.Key, SqlDbType.Xml)
                        {
                            Value = new SqlXml(new XmlTextReader(param.Value.ToString(), XmlNodeType.Document, null))
                        };
                        comm.Parameters.Add(xml);
                    }
                    else
                        comm.Parameters.AddWithValue(param.Key, param.Value);

                });

                using (var reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(reader.ToDictionary());
                }
            }

            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// bulk loads data into a table
        /// </summary>
        /// <param name="table">sql table name</param>
        /// <param name="rawData">the raw data</param>
        /// <param name="columnMap">column mapping from the data keys to the table column</param>
        /// <returns></returns>
        public int BulkLoad(string table, IEnumerable<string[]> rawData, IDictionary<string, Type> columnMap)
        {
            var rows = new DataTable(table);
            
            var bulkCopy = new SqlBulkCopy(_connection) {DestinationTableName = table};
            //map our columns
            foreach (var column in columnMap)
            {
                rows.Columns.Add(column.Key, column.Value);
                bulkCopy.ColumnMappings.Add(column.Key, column.Key);
            }

            //add our data
            foreach (var row in rawData)
                rows.Rows.Add(row);

            bulkCopy.WriteToServer(rows);

            return rawData.Count();
        }


        public IEnumerable<IDictionary<string, object>> TableValueParameter(string sql, IEnumerable<string[]> rawData,
            IDictionary<string, Type> columnMap, string parameter)
        {
            var rows = new DataTable();
            var result = new List<IDictionary<string, object>>();

            //map our columns
            foreach (var column in columnMap)
                rows.Columns.Add(column.Key, column.Value);

            //add our data
            foreach (var row in rawData)
                rows.Rows.Add(row);

            using (var comm = new SqlCommand(sql, _connection))
            {
                comm.CommandType = CommandType.StoredProcedure;
                var tvp = new SqlParameter(parameter, SqlDbType.Structured) {Value = rows};
                comm.Parameters.Add(tvp);

                using (var reader = comm.ExecuteReader())
                {
                    while (reader.Read())
                        result.Add(reader.ToDictionary());
                }
            }

            return result;
        }
    }
}

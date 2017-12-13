using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace System.Data.SqlClient
{
    /// <summary>
    /// 数据库的一个打开的连接扩展类
    /// </summary>
    public static class ExtSqlConnection
    {
        /// <summary>
        /// EF(SqlConnection)db.Database.Connection SQL 语句返回 dataTable
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="sql">sql语句</param>
        /// <returns>DataTable</returns>
        public static DataTable ExtSqlQuery(this SqlConnection conn, string sql)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable table = new DataTable();
            adapter.Fill(table);
            conn.Close();
            conn.Dispose();
            return table;
        }
    }
}

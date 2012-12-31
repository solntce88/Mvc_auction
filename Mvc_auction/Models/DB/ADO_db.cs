using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace Mvc_auction.Models.DB
{
    public class ADO_db
    {
        static public void ExeProcedure(string connString, string storedProcName, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    cmd.Connection.Open();
                    int p = cmd.ExecuteNonQuery();
                }

            }
        }
        static public void ExeProcedure(string connString, string storedProcName, SqlParameter parameter)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(storedProcName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(parameter);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        static public void ModifyData(string connString, string sqlQuary)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuary, conn))
                {
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
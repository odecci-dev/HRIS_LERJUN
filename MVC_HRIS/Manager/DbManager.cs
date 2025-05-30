﻿using Microsoft.Data.SqlClient;
using System.Data;
using MVC_HRIS.Manager;
using MVC_HRIS.Models;

namespace API_HRIS.Manager
{
    public class DbManager
    {
        IConfiguration config = new ConfigurationBuilder()
          .SetBasePath(Path.GetPathRoot(Environment.SystemDirectory))
          .AddJsonFile("app/hris/appconfig.json", optional: true, reloadOnChange: true)
          .Build();

        public SqlConnection Connection { get; set; }
        public SqlConnection conn = new SqlConnection();
        public SqlCommand cmd = new SqlCommand();
        public SqlDataAdapter da = new SqlDataAdapter();
        string cnnstr = "";
        DBConn db = new DBConn();

       
        public void ConnectioStr()
        {
            var url = config["ConnectionStrings:Hris_Constrings"];
            conn = new SqlConnection(url);
        }
        public DataSet SelectDb(string value)
        {
            DataSet ds = new DataSet();
            try
            {
                ConnectioStr();
                SQLConnOpen();
                cmd.CommandTimeout = 0;
                cmd.CommandText = value;
                da.SelectCommand = cmd;
                da.Fill(ds);

            }
            catch (Exception e)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add(new Object[] { e.Message });
                ds.Tables.Add(dt);
            }

            conn.Close();
            conn = null;
            return ds;
        }

      

        public void SQLConnOpen()
        {
            
            if (conn.State != ConnectionState.Closed) conn.Close();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }

      
        public DataSet SelectDb_SP(string strSql, params IDataParameter[] sqlParams)
        {
            DataSet ds = new DataSet();
            int ctr = 0;
        retry:
            try
            {
                ConnectioStr();
                SqlCommand cmd = new SqlCommand(strSql, conn);

                conn.Open();

                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                if (sqlParams != null)
                {

                    foreach (IDataParameter para in sqlParams)
                    {
                        SqlParameter nameParam = new SqlParameter(para.ParameterName, para.Value);
                        cmd.Parameters.Add(nameParam);
                    }
                }
                da.SelectCommand = cmd;
                da.Fill(ds);
                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                if (ctr <= 3)
                {
                    Thread.Sleep(1000);
                    ctr++;
                    goto retry;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add(new Object[] { ex.Message });
                ds.Tables.Add(dt);
            }

            conn.Close();
            return ds;
        }
        public string AUIDB_WithParam(string strSql, params IDataParameter[] sqlParams)
        {
            try
            {
                ConnectioStr();
                SqlCommand cmd = new SqlCommand(strSql, conn);

                conn.Open();

                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                if (sqlParams != null)
                {
                    foreach (IDataParameter para in sqlParams)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                cmd.ExecuteNonQuery();
                conn.Close();
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message + "!";
            }
        }
    }
}

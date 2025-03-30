
using System.Data;
using API_HRIS.Manager;
using API_HRIS.Models;
using System.Text.Json;
using API_HRIS.Manager;
using System.Data.SqlClient;

namespace API_HRIS.Manager
{
    public class DbManager
    {
        public SqlConnection Connection { get; set; }
        public SqlConnection conn = new SqlConnection();
        public SqlCommand cmd = new SqlCommand();
        public SqlDataAdapter da = new SqlDataAdapter();
        string cnnstr = "";
        DBConn db = new DBConn();
        public string DB_WithParam(string strSql, params IDataParameter[] sqlParams)
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
                //   cmd.ExecuteNonQuery();
                int rowsaffected = cmd.ExecuteNonQuery();
                conn.Close();
                string filePath = @"C:\data\SQL_Error.json"; // Replace with your desired file path
                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(rowsaffected + " Successfully"));
                return rowsaffected + " Successfully";

            }
            catch (SqlException ex)
            {
                conn.Close();
                conn = null;
                string filePath = @"C:\data\SQL_Error.json"; // Replace with your desired file path

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(ex.Message + "!"));
                return ex.Message + "!";
            }
        }
        //public bool InitializeConnection()
        //{
        //    bool isSuccess = false;
        //    try
        //    {
        //        this.Connection = new SqlConnection(DBConn.ConnectionString);
        //        this.Connection.Open();
        //        isSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        throw ex;
        //    }
        //    finally
        //    {
        //        this.Connection.Close();
        //    }
        //    return isSuccess;
        //}
        public void ConnectioStr()
        {
            // cnnstr = "Data Source=DESKTOP-9P0BJ07;Initial Catalog=ODC_HRIS;User ID=test;Password=1234";
             //cnnstr = "Data Source=LAPTOP-SULGD5RQ;Initial Catalog=ODC_HRIS;User ID=test;Password=1234";

            ////cnnstr = "Data Source=EC2AMAZ-V52FJK1;Initial Catalog=ODC_HRIS;User ID=test;Password=1234"; //  odc-hris

            cnnstr = "Data Source=LAPTOP-3191GBJB\\SQLEXPRESS;Initial Catalog=ODC_HRIS;User ID=test;Password=1234;"; // France
            //cnnstr = "Data Source=EC2AMAZ-V52FJK1;Initial Catalog=ODC_HRIS;User ID=test;Password=1234"; //  odc-hris
            conn = new SqlConnection(cnnstr);
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

        //public string AUIDB(string strSql)
        //{
        //    string result = "";
        //    int ctr = 0;
        //retry:
        //    try
        //    {
        //        InitializeConnection();
        //        SqlCommand cmd = new SqlCommand(strSql, conn);

        //        conn.Open();

        //        cmd.Connection = conn;
        //        cmd.CommandTimeout = 0;
        //        cmd.CommandType = CommandType.Text;
        //        cmd.ExecuteNonQuery();
        //        conn.Close();
        //        result = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        ctr += 1;
        //        result = ex.Message + "!";

        //        if (ctr <= 3)
        //        {
        //            goto retry;
        //        }

        //    }
        //    return result;
        //}
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
                //   cmd.ExecuteNonQuery();
                int rowsaffected = cmd.ExecuteNonQuery();
                conn.Close();
                return rowsaffected + " Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message + "!";
            }
        }
    }
}

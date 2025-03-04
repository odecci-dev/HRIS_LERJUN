using MVC_HRIS.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API_HRIS.Manager
{
    public class DBMethods
    {
        string sql = "";
        DbManager db = new DbManager();

        public string InsertAuditTrail(string actions, string datecreated, string module, string name, string userid, string read,  string employeeid)
        {
           string query = $@"insert into tbl_audittrailModel (Actions,Module,UserId,status,EmployeeID,ActionID,Business,DateCreated) values 
                ('" + actions + "'," +
                "'" + module + "'," +
                "'" + userid + "'," +
                "'" + read + "'," +
                "'" + employeeid + "'," +
                "''," +
                "''," +
                "'" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')";
            return db.AUIDB_WithParam(query);
        }
    }
}

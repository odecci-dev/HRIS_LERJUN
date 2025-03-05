namespace MVC_HRIS.Manager
{
    public class DBConn
    {
        private static string GetConnectionString()
        {
            //Return My.Settings.ConnString.ToString
            //return "Data Source=192.168.0.84,36832;Initial Catalog=EQMS;User ID=randy;Password=otik"; //test
            //return "Data Source=192.168.0.222,36832;Initial Catalog=EQMS;User ID=randy;Password=otik"; //live
            //return "Data Source=LERJUN-PC;Initial Catalog=AOPCDB;User ID=test;Password=1234"; //live
            //return "Data Source=EC2AMAZ-AN808JE\\MSSQLSERVER01;Initial Catalog=AOPCDB;User ID=test;Password=1234"; //server
            return "Data Source=LAPTOP-3191GBJB\\SQLEXPRESS;Initial Catalog=ODC_HRIS;User ID=test;Password=1234;"; // France
        }
        private static string GetHttpString()
        {
            //return "http://ec2-54-251-135-135.ap-southeast-1.compute.amazonaws.com:8089"; // live AOPC
            return "http://localhost:64181"; // local live
          // return "http://localhost:8089"; // local live
            //return "http://ec2-54-251-135-135.ap-southeast-1.compute.amazonaws.com:8089"; //ODC-HRIS
        }

        private static string GetPath()
        {

            // return    "C:\\Files\\";
            //return "/img/";
            //return "C:\\MVC_HRIS_IIS\\wwwroot\\img\\";
            return "C:\\Users\\franc\\Documents\\C# Project\\Odecci\\ODC-HRIS-20250103\\ODC-HRIS\\MVC_HRIS\\wwwroot\\Uploads";
            //return "C:\\inetpub\\HRIS-FRANCE\\ODC-HRIS\\MVC_HRIS\\wwwroot\\Uploads";
            //return "C:\\inetpub\\HRIS-FRANCE\\CMS-ODC-HRIS\\wwwroot\\img";
        }
        public static string ConnectionString
        {
            get
            {
                return GetConnectionString();
            }
        }
        public static string Path
        {
            get
            {
                return GetPath();
            }
        }

        public static string HttpString
        {
            get
            {
                return GetHttpString();
            }
        }
    }
}

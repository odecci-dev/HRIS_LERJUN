namespace API_HRIS.Manager
{
    public class DBConn
    {
        private static string GetPath()
        {
            //return "C:\\Files\\"; //Filepath

            return "C:\\Users\\franc\\Documents\\C# Project\\Odecci\\ODC-HRIS-20250103\\ODC-HRIS\\MVC_HRIS\\wwwroot\\Uploads";

            //return "C:\\inetpub\\HRIS-FRANCE\\CMS-ODC-HRIS\\wwwroot\\img";
        }
        public static string Path
        {
            get
            {
                return GetPath();
            }
        }
    }
}

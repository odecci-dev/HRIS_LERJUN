using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MVC_HRIS.Manager
{
    public class DBConn
    {
        private static IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Path.GetPathRoot(Environment.SystemDirectory)) 
            .AddJsonFile("app/hris/appconfig.json", optional: true, reloadOnChange: true)
            .Build();
        public string GetHttpString()
        {
            return config["HRIS:baseurl"];
        }

        public string GetPathx()
        {
            return config["HRIS:HrisPath"];
        }
        public static string Paths => config["HRIS:HrisPath"];

        public static string HttpString => config["HRIS:baseurl"];
    }
}

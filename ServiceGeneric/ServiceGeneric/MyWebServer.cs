using CatchException;
using EmbeddedWebServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGenerique
{
    public class MyWebServer : WebServerEngine
    {

        static WebServerConfiguration webConf = null;

        private MyWebServer(WebServerConfiguration webConf)
            : base(webConf)
        {
        }


        private static MyWebServer _Instance = null;


        public static MyWebServer Instance
        {
            get
            {
                if (_Instance == null)
                {

                    string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);


                    webConf = new WebServerConfiguration();

                    webConf.ServerName = Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
                    webConf.IPAddress = System.Net.IPAddress.Any;
                    webConf.Port = 8500 + (CatchMe.TheApplicationId.HasValue ? CatchMe.TheApplicationId.Value : 85);
                    // Path to the server root
                    webConf.ServerRoot = currentPath;
                    webConf.AddDefaultFile("default.aspx");
                    webConf.AddMimeType(".htm", "text/html");
                    webConf.AddMimeType(".html", "text/html");
                    webConf.AddMimeType(".png", "image/png");
                    webConf.AddMimeType(".jpg", "image/jpg");
                    webConf.AddMimeType(".jpeg", "image/jpg");
                    webConf.AddMimeType(".gif", "image/gif");
                    webConf.AddMimeType(".bmp", "image/bmp");
                    webConf.AddMimeType(".cgi", "text/html");
                    webConf.AddMimeType(".log", "text/xml");
                    webConf.AddMimeType(".axp", "text/html");
                    webConf.AddMimeType(".ico", "image/vnd.microsoft.icon");
                    webConf.AddMimeType(".css", "text/css");
                    webConf.AddMimeType(".gzip", "application/x-gzip");
                    webConf.AddMimeType(".zip", "multipart/x-zip");
                    webConf.AddMimeType(".tar", "application/x-tar");
                    webConf.AddMimeType(".pdf", "application/pdf");
                    webConf.AddMimeType(".xls", "application/vnd.ms-excel");
                    webConf.AddMimeType(".js", "application/javascript");

                    webConf.AddModule(new WebModule<AspxModule>(".aspx"));

                    _Instance = new MyWebServer(webConf);
                    _Instance.Start();
                }

                return _Instance;
            }
        }
    }
}

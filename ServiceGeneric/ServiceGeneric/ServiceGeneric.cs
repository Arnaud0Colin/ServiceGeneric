using CatchException;
using Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceGenerique
{
    public class ServiceGeneric<T,U> : ServiceBase where T : ConfigGeneric<T> where U : StatGeneric<U> , new()
    {
        protected Mutex mut = new Mutex();
        FileSystemWatcher watcher = new FileSystemWatcher();
        MyWebServer MyServer = null;
        protected Timer TaskTimer = null;

        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        public string Titre { get; private set; }
        

//        private string m_Titre = null;
        protected bool  IsNotifyEvent = false;

        public string m_PathLog = null;
        public string PathLog
        {
            get
            {
                if( m_PathLog != null)
                    return  m_PathLog;

                string currentPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
                return currentPath + @"\Temp\";
            }
            set
            {
                if (Directory.Exists(value))
                    m_PathLog = value;
                else
                    m_PathLog = null;
            }
        }

         ~ServiceGeneric()
        {
         //   MyServer.Stop();
        }

        public ServiceGeneric( int ID, string Titre )
        {
           this.Titre = Titre;

          CatchMe.TheApplicationId = ID;
            

#if !DEBUG
            CatchMe.CatchUnhandled();
#endif
            MyServer = MyWebServer.Instance;

            GetFileName();
            OnFileChanged(null, null);

            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
| NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Path = AssemblyDirectory;
            watcher.Filter = FileName;
            watcher.Changed += new FileSystemEventHandler(OnFileChanged);
            watcher.EnableRaisingEvents = true;

        }

     
       

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private string FileName = null;
        private void GetFileName()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(ConfigFile), false);
            if (attributes.Length == 1)
            {
                this.FileName = ((ConfigFile)attributes[0]).FileName;
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            mut.WaitOne();
            try
            {
                T config = CXmlFile.DeserializeFromXml<T>(AssemblyDirectory + "\\" + FileName);
                ConfigGeneric<T>.Instance = config;
                InitServerLogSmtp();
            }
            catch (Exception ex)
            {
                Smtp.NotificationSmtp .ServeurInfo = null;
                CatchMe.WriteException(ex).Where().Write();
            }
            mut.ReleaseMutex();
        }

        protected virtual void InitMailLogSmtp()
        {
            Smtp.NotificationSmtp.MessageSmtp = new Smtp.MessageSmtp($@"{Titre}@Test.zz", new string[] {@"colin@fransbonhomme.fr" } , Titre);
        }



        private void InitServerLogSmtp()
        {
            InitMailLogSmtp();

            System.Net.NetworkCredential credential = null;

            if(ConfigGeneric<T>.Instance.SMTP.Login != null)
            {
                credential = new System.Net.NetworkCredential(ConfigGeneric<T>.Instance.SMTP.Login, ConfigGeneric<T>.Instance.SMTP.Password);
            }

            Smtp.NotificationSmtp.ServeurInfo = new Smtp.ServeurInfo()
            {
                server = ConfigGeneric<T>.Instance.SMTP.Name,
                Credential = credential
            }; 
        }


        protected override void OnStart(string[] args)
        {
            if( IsNotifyEvent )
                CatchMe.WriteException($"Service {Titre} Start").Level(1).Where().Write();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            if (TaskTimer != null)
            {
                TaskTimer.Dispose();
                TaskTimer = null;
            }

            if (IsNotifyEvent)
                CatchMe.WriteException($"Service {Titre} Stop").Level(1).Where().Write();

            base.OnStop();
        }

        protected override void OnShutdown()
        {
            if (TaskTimer != null)
            {
                TaskTimer.Dispose();
                TaskTimer = null;
            }

            if (IsNotifyEvent)
                CatchMe.WriteException($"Service {Titre} Shutdown").Level(1).Where().Write();

            base.OnShutdown();
        }



       public  /*protected*/ void CalculDelay(DateTime date, bool bInit)
        {
            var config = ConfigGeneric<T>.Instance;
            var Stat = StatGeneric<U>.Instance;


            if ( !config.CheckDelay || !config.CheckTime)
            {
                Stat.Sleep = true;
                if (config.ExplicitTrace > 1)
                    TraceFile.WriteLine($"Changement de mode [Sommeil éternelle]");
                return;
            }

            int iSaveDelay = Stat.Delay;
            int time = config.Delay * 60;
            bool bSave = Stat.Sleep;


            bool fin = config.HeureFin < date;


            int today = (int)date.DayOfWeek;
            int i = today;
            int count = 0;
            while (!config.Jours[i] || fin)
            {
                i++;
                count++;
                if (i == 7) i = 0;
                if (count > 7) break;
                fin = false;
            }

            if (count > 0)
            {
                time = ((count - 1) * 60 * 24 * 60) + (date - new Time(23, 59)) + 60 + config.HeureDebut;
                Stat.Sleep = true;
            }
            else
            if (config.HeureFin < date)
            {
                time = date - config.HeureDebut;
                Stat.Sleep = true;
            }
            else
                if (config.HeureDebut > date)
            {
                time = date - config.HeureDebut;
                Stat.Sleep = true;
            }
            else
                    if (config.PauseMidi && (config.HeureMidiFin > date) && (config.HeureMidiDebut < date))
            {
                time = date - config.HeureMidiFin;
                Stat.Sleep = true;
            }
            else
                Stat.Sleep = false;

            if (TaskTimer == null)
            {
                if (!Stat.Sleep)
                    TaskTimer = new Timer(RunMethod, autoEvent, 0, 1000 * 60 * config.Delay);
                else
                    TaskTimer = new Timer(RunMethod, autoEvent, 1000 * time, 1000 * 60 * config.Delay);
            }
            else
                if (bSave != Stat.Sleep)
            {

                if (config.ExplicitTrace > 4)
                    TraceFile.WriteLine($"Changement de mode [{(Stat.Sleep ? "Sommeil" : "Normal")}] : {time / 60} Minute(s) ");

                TaskTimer.Change(1000 * time, 1000 * 60 * config.Delay);
            }
            else
            {
                if (time < config.Delay)
                {
                    TaskTimer.Change(1000 * time, 1000 * config.Delay);

                    if (config.ExplicitTrace > 5)
                        TraceFile.WriteLine($"Mode Actuelle [{(Stat.Sleep ? "Sommeil" : "Normal")}] : {time / 60} Minute(s) vieux delai {iSaveDelay / 60} Minute(s) ");
                }

            }

            Stat.Delay = time;

        }

        public virtual /*private*/ void RunMethod(object state)
        {

        }
     

    }
}

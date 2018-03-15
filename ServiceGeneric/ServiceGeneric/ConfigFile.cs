using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceGenerique
{

      public class Host
    {
        public string Name;
        public string Login;
        public string Password;
        public int Port;
    }

    [Flags]
    public enum Jour
    {
        Lundi,
        Mardi,
        Mercredi,
        Jeudi,
        Vendredi,
        Samedi,
        Dimanche,

    }

    public class ConfigGeneric<T>
    {
        public static T Instance = default(T);      
        public Host SMTP = null;

        public int Delay;
        public Time HeureDebut;
        public Time HeureMidiDebut;
        public Time HeureMidiFin;
        public Time HeureFin;

        public bool[] Jours = new bool[7];

        public ushort ExplicitTrace = 4;

        [XmlIgnore]
        public bool PauseMidi
        {
            get { return HeureMidiDebut != null && HeureMidiFin != null; }
        }


        public string GetDelay() => $"{Delay} Minute(s)";
        public string GetTimeAM() => $" de {HeureDebut}  {(HeureMidiDebut != null ?  $"a {HeureMidiDebut}" : $"")} ";
        public string GetTimePM() => $" {(HeureMidiFin != null ? $"de {HeureMidiFin}" : $"")} a {HeureFin} ";
        

        [XmlIgnore]
        public bool IsValidTime
        {
            get
            {
                return (HeureDebut < HeureFin) &&
                  ( !PauseMidi || (PauseMidi &&
                    HeureDebut < HeureMidiDebut &&
                    HeureMidiDebut < HeureMidiFin &&
                    HeureMidiFin < HeureFin)); 
            }
        }

        [XmlIgnore]
        public bool IsValidDelay
        {
            get
            {
                return Delay > 3;
            }
        }


        [XmlIgnore]
        public bool CheckTime
        {
            get
            {
                bool valid = IsValidTime;

                if (!valid)
                {
                    CatchException.TraceFile.WriteLine("config is Time invalid");
                    CatchException.CatchMe.WriteException("config is Time invalid").Where()
                                       .Variable("HeureDebut", HeureDebut)
                                       .Variable("HeureMidiDebut", HeureMidiDebut)
                                       .Variable("HeureMidiFin", HeureMidiFin)
                                       .Variable("HeureFin", HeureFin)
                                       .Write();
                }

                return valid;
            }
        }

        [XmlIgnore]
        public bool CheckDelay
        {
            get
            {
                bool valid = IsValidDelay;
                if (!valid)
                {
                    CatchException.TraceFile.WriteLine("config is Delay invalid");
                    CatchException.CatchMe.WriteException("config is Delay invalid").Where()
                                       .Variable("Delay", Delay)
                                       .Write();
                }

                return valid;
            }
        }


      

    }

    /// <summary>
    /// Number Of Digit
    /// <example>  </example>/// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ConfigFile : Attribute
    {
        /// <summary>
        /// <para>
        /// <param name="nb">ushort</param>
        /// </para>
        /// Constructor
        /// </summary>
        public ConfigFile(string File)
        {
            this.FileName = File;
        }

        /// <summary>
        /// Number of digit
        /// <example>  </example>/// 
        /// <returns>ushort</returns>
        /// </summary>
        public string FileName { get; private set; }
    }
}

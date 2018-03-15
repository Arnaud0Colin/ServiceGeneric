using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGenerique
{
    public class StatGeneric<T> where T : new()
    {
        public readonly static T Instance = new T();

        public bool Sleep = false;
        public DateTime NextRun;
        public DateTime BootTime;
        public int CountBoucle = 0;
        public int CountError = 0;
        public int CountBoucleSleep = 0;

        private DateTime _DelayChanged = default(DateTime);

        public bool DoubleError()
        {
            bool result = ((SaveOnErrorCountBoucle + 1) == CountBoucle);
            SaveOnErrorCountBoucle = CountBoucle;
            return result;
        }
        private int SaveOnErrorCountBoucle = 0;

        public string DelayChanged
        {
            get
            {
                if (_DelayChanged == default(DateTime))
                    return "Jamais";
                else
                    return _DelayChanged.ToString();
            }
        }

        public string DelayExpire
        {
            get
            {
                if (_DelayChanged == default(DateTime))
                    return "Jamais";
                else
                    if (_Delay > 0)
                    return _DelayChanged.AddSeconds(_Delay).ToString();
                else
                    return "Jamais";
            }
        }

        public bool isNeedColorDelayExpire
        {
            get
            {
                if (_DelayChanged != default(DateTime) && _Delay > 0)
                {
                    return (_DelayChanged.AddSeconds(_Delay) > DateTime.Now);
                }
                else
                    return false;
            }
        }


        private int _Delay = 0;
        public int Delay
        {
            get
            {
                return _Delay;
            }
            set
            {
                _Delay = value;
                _DelayChanged = DateTime.Now;
            }
        }

        public TimeSpan Rest
        {
            get
            {
                if (NextRun == default(DateTime))
                    return new TimeSpan(0, 0, Delay);
                else
                    return new TimeSpan(0, 0, Delay) - (DateTime.Now - NextRun);
            }
        }

        public TimeSpan Uptime
        {
            get
            {
                if (BootTime == default(DateTime))
                    return new TimeSpan(0);
                else
                    return DateTime.Now - BootTime;
            }
        }

    }
}
